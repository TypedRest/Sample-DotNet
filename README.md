# TypedRest Address Book Sample

This is a sample project for using [TypedRest for .NET](https://github.com/TypedRest/TypedRest-DotNet). It provides a simple REST API for storing contacts in an address book and uses SQLite for persistance.

The code is split into:

- [Service/](Service/): an [ASP.NET Web API](https://dotnet.microsoft.com/apps/aspnet/apis) Service
- [Client/](Client/): a TypedRest client library
- [Dto/](Dto/): DTOs shared by the Client and the Service
- [UnitTests/](UnitTests/): Tests that ensure the Client and the Service work together
