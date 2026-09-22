# Setup

How to get RootCause running locally from a clean clone: four containerised services, and the
Stack Overflow dataset the measurements are taken against.

---

## Prerequisites

| | |
|---|---|
| **Docker Desktop** | 28.x or later |
| **Rosetta** | Required on Apple silicon — SQL Server publishes no arm64 image |
| **Disk** | ~55 GB for the dataset, plus room for the container volumes |
| **Memory** | Give Docker at least 8 GB. SQL Server alone wants 2 GB before it will start |

> **Enable Rosetta** in Docker Desktop under *Settings → General → Use Rosetta for x86_64/amd64
> emulation on Apple Silicon*. Without it the SQL Server container will not start.

---

## The services

```bash
docker compose up -d
docker compose ps        # all four should read "healthy", not merely "running"
```

| Service | Image | Host port | Ready when |
|---|---|---|---|
| **sqlserver** | `mcr.microsoft.com/mssql/server:2025-latest` | 1433 | `sqlcmd … -Q 'SELECT 1'` returns 0 |
| **postgres** | `postgres:16` | 5432 | `pg_isready` returns 0 |
| **redis** | `redis:7-alpine` | 6379 | `redis-cli ping` returns PONG |
| **rabbitmq** | `rabbitmq:4-management` | 5672, 15672 | `rabbitmq-diagnostics -q ping` returns 0 |

RabbitMQ's management page is at <http://localhost:15672>.

**SQL Server is pinned to `linux/amd64`** because no arm64 image exists. It runs under emulation on
Apple silicon.

> ⚠️ **Never quote a duration measured in that container as a result.** Emulation distorts
> wall-clock timing. Logical reads, row counts and plan shapes are unaffected and are what this
> project measures.

SQL Server takes 60–90 seconds to become healthy on first start. That is why its health check has
`start_period: 90s` — without it the container would be marked unhealthy before it ever had a
chance.

### Secrets

The `sa` password is read from `.env`, which is **not** committed:

```bash
echo 'MSSQL_SA_PASSWORD=<your password>' > .env
```

It must be at least 8 characters with upper case, lower case, a digit and a symbol, or the
container refuses to start. Compose fails with a clear message if the variable is missing.

---

## Connecting

**The same database has two addresses**, and mixing them up is the most common confusion:

| Connecting from | Server |
|---|---|
| A service **inside** Compose | `sqlserver,1433` — containers find each other by service name |
| Azure Data Studio **on your Mac** | `localhost,1433` |

Log in as `sa` with the password from `.env`. Azure Data Studio needs **Trust server certificate**
ticked, because the container's certificate is self-signed.

---

## The dataset

**Stack Overflow — Mini 2013**, imported from the Stack Exchange Data Dump and distributed by
Brent Ozar Unlimited. Provided under **CC BY-SA 3.0**, attributed to the original authors at
<https://archive.org/details/stackexchange>. Download: <http://www.brentozar.com/go/querystack>

The `data/` folder is **git-ignored** — it is ~55 GB. A fresh clone downloads it separately.

### Attaching it

Extract the archive into `data/`, which Compose mounts at `/data` inside the container. Then:

```sql
CREATE DATABASE StackOverflow2013 ON
  (FILENAME = '/data/StackOverflow2013_1.mdf'),
  (FILENAME = '/data/StackOverflow2013_2.ndf'),
  (FILENAME = '/data/StackOverflow2013_3.ndf'),
  (FILENAME = '/data/StackOverflow2013_4.ndf'),
  (FILENAME = '/data/StackOverflow2013_log.ldf')
FOR ATTACH;
```

> ⚠️ **All five files must be listed.** The dataset is one `.mdf`, **three `.ndf`** and one `.ldf`.
> Almost every attach example online shows only `mdf` + `ldf`, and the paths recorded inside the
> `.mdf` are the original Windows ones, so SQL Server cannot find the others by itself.

Copying the files into the folder does **not** create a database — SQL Server never scans
directories. Until the statement above runs, Azure Data Studio shows the server with no user
databases on it.

---

## What is in it

Recorded 21 September 2026, immediately after attaching.

### Rows

| Table | Rows |
|---|---|
| Votes | 52,928,720 |
| Comments | 24,534,730 |
| Posts | 17,142,169 |
| Badges | 8,042,005 |
| Users | 2,465,713 |
| PostLinks | 1,421,208 |
| VoteTypes | 15 |
| PostTypes | 8 |
| LinkTypes | 2 |

### On disk

| File | Type | Size |
|---|---|---|
| `StackOverflow2013_1` | ROWS | 12.70 GB |
| `StackOverflow2013_2` | ROWS | 12.70 GB |
| `StackOverflow2013_3` | ROWS | 12.70 GB |
| `StackOverflow2013_4` | ROWS | 12.70 GB |
| `StackOverflow2013_log` | LOG | 0.24 GB |
| **Total** | | **50.8 GB** |

### Indexes

**9 clustered indexes. Zero nonclustered.**

That is deliberate on the dataset's part, and it is the reason this dataset was chosen:
**every index in any before-and-after demonstration is one this project added.** Nothing is staged.

---

## Server and compatibility level

| | |
|---|---|
| Product version | **17.0.5005.3** (SQL Server 2025) |
| Edition | Enterprise Developer |
| `StackOverflow2013` compatibility level | **100** |
| Recovery model | SIMPLE |

🔥 **The database attaches at compatibility level 100, not 170, and that is left alone
deliberately.**

The dataset is in SQL Server 2008 format. SQL Server 2025 still supports level 100, so nothing
raises it. The result is a **2025 engine optimising like 2008** — which is precisely the condition
this analyser detects from a plan file alone, by reading `CardinalityEstimationModelVersion`.

The test data is the demonstration. Plans captured at level 100 are the hardest fixtures to
reproduce later, so capture those before changing anything.

Compatibility level is per-database and changes instantly, so one database covers every optimiser
generation:

```sql
SELECT name, compatibility_level FROM sys.databases;

ALTER DATABASE StackOverflow2013 SET COMPATIBILITY_LEVEL = 160;   -- 100 … 170 all supported
```

Changing it clears that database's cached plans, so the next execution recompiles — which is what
you want when capturing a fresh plan at a new level. **Record the level in every fixture's
filename**, e.g. `sniffing-ce100.sqlplan`.

---

## Before any index work: grow the log

The dataset ships with a 0.24 GB log file, and the publisher's readme says to grow it before
modifying data. Creating indexes on a 17-million-row table will otherwise trigger repeated
autogrowth and distort every measurement taken while it happens.

```sql
ALTER DATABASE StackOverflow2013
  MODIFY FILE (NAME = 'StackOverflow2013_log', SIZE = 8GB, FILEGROWTH = 512MB);
```

---

## Troubleshooting

| Symptom | Cause and fix |
|---|---|
| `port is already allocated` | Something else holds 1433 or 5432. `lsof -i :1433` to find it, or change the left-hand number in `ports:` |
| SQL Server container exits immediately | `ACCEPT_EULA` arrived as something other than the string `"Y"`, or the password fails the complexity rule. Check `docker compose logs sqlserver` |
| Health check never passes | Verify the tools path exists: `docker compose exec sqlserver ls /opt/`. The image must have `mssql-tools18`; older images use `mssql-tools` |
| Attach fails with OS error 5, 31 or 38 | The bind mount cannot support the IO SQL Server needs. Copy the files into the container's own volume and attach from there: `docker compose exec sqlserver bash -c "cp /data/StackOverflow2013_* /var/opt/mssql/data/"` |
| Server connects but no databases are listed | The database was never attached. See *Attaching it* above, and refresh the Databases node — Azure Data Studio does not poll |
