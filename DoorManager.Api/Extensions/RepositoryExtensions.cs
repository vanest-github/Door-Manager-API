using DoorManager.Storage.Interface.Commands;
using DoorManager.Storage.Interface.Queries;
using DoorManager.Storage.MySql;
using DoorManager.Storage.MySql.Repositories.Commands;
using DoorManager.Storage.MySql.Repositories.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Api.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositorties(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DoorManagerDbContext>(x => x.UseMySQL(configuration.GetConnectionString("AzureHost")), ServiceLifetime.Singleton);
        AddCommandRepositorties(services);
        AddQueryRepositorties(services);
    }

    public static void AddCommandRepositorties(this IServiceCollection services)
    {
        services.AddSingleton<IActivityCommandRepository, ActivityCommandRepository>();
        services.AddSingleton<IDoorAccessCommandRepository, DoorAccessCommandRepository>();
        services.AddSingleton<IDoorCommandRepository, DoorCommandRepository>();
        services.AddSingleton<IUserOfficeRoleCommandRepository, UserOfficeRoleCommandRepository>();
        services.AddSingleton<IUserCommandRepository, UserCommandRepository>();
        services.AddSingleton<IOfficeCommandRepository, OfficeCommandRepository>();
        services.AddSingleton<IRoleCommandRepository, RoleCommandRepository>();
    }

    public static void AddQueryRepositorties(this IServiceCollection services)
    {
        services.AddSingleton<IActivityQueryRepository, ActivityQueryRepository>();
        services.AddSingleton<IDoorAccessQueryRepository, DoorAccessQueryRepository>();
        services.AddSingleton<IDoorQueryRepository, DoorQueryRepository>();
        services.AddSingleton<IUserOfficeRoleQueryRepository, UserOfficeRoleQueryRepository>();
        services.AddSingleton<IUserQueryRepository, UserQueryRepository>();
        services.AddSingleton<IOfficeQueryRepository, OfficeQueryRepository>();
        services.AddSingleton<IRoleQueryRepository, RoleQueryRepository>();
        services.AddSingleton<IRBACQueryRepository, RBACQueryRepository>();
    }
}
