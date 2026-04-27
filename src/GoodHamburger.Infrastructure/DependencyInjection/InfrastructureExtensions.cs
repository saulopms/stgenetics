using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;
using GoodHamburger.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IMenuCatalog, MenuCatalog>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        return services;
    }
}
