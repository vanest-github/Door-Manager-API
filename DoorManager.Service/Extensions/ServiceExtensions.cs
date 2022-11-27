using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DoorManager.Service.Extensions;

public static class ServiceExtensions
{
    public static void AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceExtensions).Assembly);
    }
}
