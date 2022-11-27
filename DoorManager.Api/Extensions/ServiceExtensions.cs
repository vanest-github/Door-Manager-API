using DoorManager.Service.Infrastructure;
using DoorManager.Service.Infrastructure.Interfaces;

namespace DoorManager.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IBus, Bus>();
    }
}
