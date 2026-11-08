# TypedRest Address Book Sample

This is a sample project for using [TypedRest for .NET](https://github.com/TypedRest/TypedRest-DotNet). It provides a simple REST API for storing contacts in an address book and uses SQLite for persistance.

The code is split into:

- [Service/](Service/): an [ASP.NET Core Web API](https://dotnet.microsoft.com/apps/aspnet/apis) Service
- [Client/](Client/): a handwritten TypedRest client library
- [GeneratedClient/](GeneratedClient/): a TypedRest client library generated from the Service's OpenAPI document
- [Dto/](Dto/): DTOs shared by the Client and the Service
- [UnitTests/](UnitTests/): Tests that ensure both clients and the Service work together

## OpenAPI

The Service describes itself with an [OpenAPI](https://www.openapis.org/) document, served at `/openapi/v1.json` together with an interactive API reference at `/scalar`.

The same document is also written to [Service/AddressBook.json](Service/AddressBook.json) on every build, so changes to the API show up in diffs.

## Generated client

The [GeneratedClient/](GeneratedClient/) project uses the [TypedRest source generator](https://typedrest.net/code-generation/source-generator/) to build a client from [Service/AddressBook.json](Service/AddressBook.json) during compilation:

```xml
<TypedRestOpenApi Include="..\Service\AddressBook.json" ServiceName="AddressBook" Serializer="system-text-json" />
```

The project depends on the Service only for build ordering, so the document is regenerated before the client is compiled. Endpoints, DTOs and a constructor taking the base URI (configured for System.Text.Json) are generated. [AddressBookClient.cs](GeneratedClient/AddressBookClient.cs) adds one more constructor in a partial class, which takes a custom `HttpClient` for testing. The generated code is written to `GeneratedClient/obj/` for inspection.

Compare it with the handwritten [Client/](Client/), which produces the same endpoint structure but shares its DTOs with the Service.
