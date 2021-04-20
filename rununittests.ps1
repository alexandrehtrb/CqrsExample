Remove-Item ".\TestResults" -Recurse -ErrorAction Ignore
dotnet test --collect:"XPlat Code Coverage" --results-directory ".\TestResults\"
reportgenerator "-reports:.\TestResults\**\coverage.cobertura.xml" "-targetdir:.\TestResults\" -reporttypes:Html