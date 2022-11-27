using Microsoft.AspNetCore.Mvc;

namespace DoorManager.Api.CustomFilters;

public class ClaimAuthorizationAttribute : TypeFilterAttribute
{
    public ClaimAuthorizationAttribute()
        : base(typeof(ClaimAuthorizationFilter))
    {
    }
}
