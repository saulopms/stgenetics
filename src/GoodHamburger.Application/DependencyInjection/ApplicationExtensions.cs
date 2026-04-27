using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application.DependencyInjection;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuService, MenuService>();
        return services;
    }
}
