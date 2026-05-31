using Application.Abstractions.Persistence;
using Infrastructure.Persistence.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IListingRepository, InMemoryListingRepository>();
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
        return services;
    }
}
