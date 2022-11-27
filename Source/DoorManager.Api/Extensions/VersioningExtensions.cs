using Microsoft.AspNetCore.Mvc;

namespace DoorManager.Api.Extensions;

public static class VersioningExtensions
{
    public static void AddApiVersionExtensions(this IServiceCollection services)
    {
        services.AddApiVersioning(v =>
        {
            v.ReportApiVersions = true;
            v.AssumeDefaultVersionWhenUnspecified = true;
            v.DefaultApiVersion = new ApiVersion(1, 0);
        });
        services.AddVersionedApiExplorer(v =>
        {
            v.GroupNameFormat = "'v'VVV";
            v.SubstituteApiVersionInUrl = true;
        });
    }
}
