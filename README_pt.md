# CQRS Example

[Read in english](README.md)

Este projeto é um exemplo de uma API de lista de compras com o intuito de mostrar alguns conceitos de arquitetura de software, principalmente CQRS (Command and Query Responsibility Segregation).

O servidor é uma ASP.NET Minimal API em .NET 10, com o mínimo possível de bibliotecas.

## Padrões de arquitetura e conceitos de programação aplicados

Os links são para estudos e referências de conhecimento.

* [Padrão CQRS](https://docs.microsoft.com/pt-br/azure/architecture/patterns/cqrs)
* [Padrão de notificações](https://martinfowler.com/articles/replaceThrowWithNotification.html)
* [Diretórios por funcionalidades](http://www.kamilgrzybek.com/design/feature-folders/)
* [Logs estruturados](https://messagetemplates.org/)
* [Tipos nuláveis e não-nuláveis](https://docs.microsoft.com/pt-br/dotnet/csharp/nullable-references)
* [Testes unitários](https://softwaretestingfundamentals.com/unit-testing/)
* [Testes de APIs](https://pororoca.io/pt/docs/automated-tests)
* [Documentação da API com exemplos](https://guides.scalar.com/scalar/scalar-api-references/net-integration)

## Para executar o projeto

O projeto requer apenas o [.NET 10](https://dotnet.microsoft.com/) instalado. A API usa um banco de dados em memória.

O script `runserverlocal.ps1` compila o projeto e executa a API, ouvindo em `https://localhost:5001`.

O script `rununittests.ps1` executa os testes unitários e gera um relatório de cobertura de testes na pasta `TestResults`. Requer o [ReportGenerator](https://github.com/danielpalme/ReportGenerator) instalado.

## NativeAOT

O projeto é preparado para compilação com trimming e em NativeAOT, através da flag `PublishAot`. Confira mais no arquivo csproj do projeto de API.
