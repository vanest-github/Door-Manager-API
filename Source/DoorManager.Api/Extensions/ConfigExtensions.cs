using DoorManager.Entity.Configurations;

namespace DoorManager.Api.Extensions;

public static class ConfigExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTConfiguration>(configuration.GetSection("JWTConfiguration"));
        services.Configure<GlobalConfiguration>(configuration.GetSection("DoorManager"));
    }
}
