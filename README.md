# CQRS Example

[Ler em português](README_pt.md)

This project is an example of a shopping list API, to show some software architectural concepts, mainly CQRS (Command and Query Responsibility Segregation).

The API is an ASP NET WebAPI using .NET 7, with as few external dependencies as possible.

## Architectural patterns and programming concepts applied

The links are for studying and for knowledge reference.

* CQRS pattern [(link1)](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) [(link2)](https://cqrs.wordpress.com/documents/cqrs-introduction/)
* Notifications pattern [(link)](https://martinfowler.com/articles/replaceThrowWithNotification.html)
* Feature folders [(link)](http://www.kamilgrzybek.com/design/feature-folders/)
* Structured logging [(link)](https://messagetemplates.org/)
* Nullable and non-nullable reference types [(link)](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references)
* Unit tests [(link)](https://softwaretestingfundamentals.com/unit-testing/)
* API tests [(link)](https://learning.postman.com/docs/writing-scripts/script-references/test-examples/)
* Swagger documentation with examples [(link)](https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters)

## To run the project

The project requires only [.NET 7](https://dotnet.microsoft.com/) installed. The API uses an in-memory database, so no database is necessary. The API testing requires [Postman](https://www.postman.com/downloads/).

The `runserverlocal.ps1` script builds and runs the API, listening on `http://localhost:5000`.

The `rununittests.ps1` script runs the unit tests and generates a testing coverage report at a `TestResults` folder. It requires the [ReportGenerator](https://github.com/danielpalme/ReportGenerator) installed.

To run the API tests, import the Postman collection and environment files located at the `tests` folder, then start the collection runner (tutorial [here](https://learning.postman.com/docs/running-collections/intro-to-collection-runs/)).