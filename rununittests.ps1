# Para instalar o Report Generator:
# To install the Report Generator:
# dotnet tool install -g dotnet-reportgenerator-globaltool
Remove-Item ".\TestResults" -Recurse -ErrorAction Ignore
dotnet test --collect:"XPlat Code Coverage" --results-directory ".\TestResults\" --filter FullyQualifiedName!~CqrsExample.Api.Tests
reportgenerator "-reports:.\TestResults\**\coverage.cobertura.xml" "-targetdir:.\TestResults\" -reporttypes:Html