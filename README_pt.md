# CQRS Example

[Read in english](README.md)

Este projeto é um exemplo de uma API de lista de compras, com o intuito de mostrar alguns conceitos de arquitetura de software, principalmente CQRS.

A API é uma ASP NET WebAPI using .NET 5, com o mínimo possível de bibliotecas.

## Padrões de arquitetura e conceitos de programação aplicados

Os links são para estudos e referências de conhecimento.

* Padrão CQRS [(link1)](https://docs.microsoft.com/pt-br/azure/architecture/patterns/cqrs) [(link2)](https://cqrs.wordpress.com/documents/cqrs-introduction/)
* Padrão de notificações (notifications pattern) [(link)](https://www.martinfowler.com/eaaDev/Notification.html)
* Diretórios por funcionalidades (feature folders) [(link)](http://www.kamilgrzybek.com/design/feature-folders/)
* Logs estruturados (structured logging) [(link)](https://messagetemplates.org/)
* Tipos nuláveis e não-nuláveis [(link)](https://docs.microsoft.com/pt-br/dotnet/csharp/nullable-references)
* Testes unitários [(link)](https://softwaretestingfundamentals.com/unit-testing/)
* Testes de APIs [(link)](https://learning.postman.com/docs/writing-scripts/script-references/test-examples/)
* Documentação Swagger com exemplos [(link)](https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters)

## Para executar o projeto

O projeto requer apenas o [.NET 5](https://dotnet.microsoft.com/) instalado. A API usa um banco de dados em memória, então não precisa de nenhum banco de dados instalado. Para executar os testes de API, é necessário ter o [Postman](https://www.postman.com/downloads/).

O script `runserverlocal.ps1` compila o projeto e executa a API, ouvindo em `http://localhost:5000`.

O script `rununittests.ps1` executa os testes unitários e gera um relatório de cobertura de testes na pasta `TestResults`.

Para rodar os testes de API, importar os arquivos de collection e environment do Postman que estão na pasta `tests` e depois iniciar o collection runner (tutorial [here](https://learning.postman.com/docs/running-collections/intro-to-collection-runs/)).