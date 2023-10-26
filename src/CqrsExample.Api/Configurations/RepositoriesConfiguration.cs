using CqrsExample.Domain.Features.Shopping.Common.Repositories;
using CqrsExample.Infrastructure.Features.Shopping.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsExample.Api.Configurations;

public static class RepositoriesConfiguration
{
    // Em uma arquitetura CQRS, a leitura é separada da escrita.
    // Aqui, isso é definido por interfaces diferentes para os repositórios: uma interface para leitura e outra interface para escrita.
    // As QueryHandlers (consultas) usam as interfaces de leitura; os CommandHandlers (comandos) usam as interfaces de escrita.
    // A arquitetura CQRS não requer bases de dados separadas para leitura e escrita, porém facilita essa implementação.

    // In a CQRS architecture, the reads are separate from the writes.
    // Here, this is defined by different interfaces for the repositories: one interface for reading and other interface for writing.
    // The QueryHandlers use the read interfaces; the CommandHandlers use the write interfaces.
    // CQRS architecture does not require different data sources for reads and writes, but eases that implementation.

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services.AddSingleton<ShoppingListInMemoryRepository>()
                .AddSingleton<IShoppingListReadRepository>(x => x.GetRequiredService<ShoppingListInMemoryRepository>())
                .AddSingleton<IShoppingListWriteRepository>(x => x.GetRequiredService<ShoppingListInMemoryRepository>());
}