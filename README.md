# CQRS Example

[Ler em português](README_pt.md)

This project is an example of a shopping list API, to show some software architectural concepts, mainly CQRS (Command and Query Responsibility Segregation).

The API is an ASP NET WebAPI using .NET 9, with as few external dependencies as possible.

## Architectural patterns and programming concepts applied

The links below are for studying and knowledge reference.

* [CQRS pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
* [Notifications pattern](https://martinfowler.com/articles/replaceThrowWithNotification.html)
* [Feature folders](http://www.kamilgrzybek.com/design/feature-folders/)
* [Structured logging](https://messagetemplates.org/)
* [Nullable type checking](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references)
* [Unit tests](https://softwaretestingfundamentals.com/unit-testing/)
* [API tests](https://pororoca.io/docs/automated-tests)
* [REST documentation](https://guides.scalar.com/scalar/scalar-api-references/net-integration)

## To run the project

The project requires only [.NET 9](https://dotnet.microsoft.com/) installed. The API uses an in-memory database, so no database is necessary.

The `runserverlocal.ps1` script builds and runs the API, listening on `https://localhost:5001`.

The `rununittests.ps1` script runs the unit tests and generates a testing coverage report at a `TestResults` folder. It requires the [ReportGenerator](https://github.com/danielpalme/ReportGenerator) installed.