# SqlAlias

[![Build status](https://ci.appveyor.com/api/projects/status/dnnn06mquuudqpkm/branch/master?svg=true)](https://ci.appveyor.com/project/droyad/sqlalias/branch/master)

SqlAlias applies alias definitions from the registry to the server portion of a Microsoft SQL Server connection string. Unlike full framework applications, .NET Core apps and websites [do not
do this](https://github.com/dotnet/corefx/issues/2575) by default.

The code is based on [user2771704's StackOverflow answer](https://stackoverflow.com/a/45330995/10784).

## Usage

Install the [SqlAlias package](https://www.nuget.org/packages/SqlAlias) from NuGet

```
using SqlAlias;

myConnectionString = Aliases.Map(myConnectionString)
```

Changes are only made to the connection string if the OS is Windows and if it is running on the .NET Core runtime.

