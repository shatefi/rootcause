# scripts

## `cpm-fix.py`

`dotnet new` is not aware of Central Package Management. Templates that ship with packages
(`xunit`, `nunit`, `mstest`, `webapi`, `worker`) write inline `Version=` attributes, which
`Directory.Packages.props` forbids — restore then fails with **NU1008**.

This moves the versions where they belong.

```bash
dotnet new xunit -o tests/Something.Tests --no-restore
python3 scripts/cpm-fix.py tests/Something.Tests/Something.Tests.csproj Directory.Packages.props
dotnet restore
```

`classlib`, `console` and `web` ship no packages, so they never need this.
`dotnet add package` is CPM-aware and needs no help.
