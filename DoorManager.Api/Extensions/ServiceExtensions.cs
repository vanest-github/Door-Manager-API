using DoorManager.Service;
using DoorManager.Service.Infrastructure;
using DoorManager.Service.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DoorManager.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IBus, Bus>();
    }
}
