# RootCause

Finds out **why** a SQL Server query is slow — and says what the fix costs, and when to leave
it alone.

Give it an execution plan and it checks the plan against a catalogue of rules, each one drawn
from a documented tuning principle rather than guesswork. Every finding carries four things:
what is happening, what to do about it, what the fix costs, and when the right answer is to do
nothing. No AI, no heuristics that cannot be explained — just the rules, written as code.

There is also a workbench side: run a query under a chosen record count and concurrency, toggle
an index or a cache on and off, and watch duration, logical reads and rows move in real time.

**Status:** in development.

## Using it

```bash
dotnet tool install -g RootCause.Cli

rootcause analyze --plan slow.sqlplan
rootcause analyze --plan slow.sqlplan --format json
```

Prefer something shorter to type? `alias rca='rootcause'`.

## Design

The interface design, its tokens and the component state sheet are in `docs/design/`.

## Data
Uses the Stack Overflow database (Mini 2013), imported from the Stack Exchange Data Dump
and distributed by Brent Ozar Unlimited. Provided under CC BY-SA 3.0, attributed to the
original authors at https://archive.org/details/stackexchange
