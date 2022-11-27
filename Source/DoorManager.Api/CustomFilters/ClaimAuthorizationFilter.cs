using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoorManager.Entity.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DoorManager.Api.CustomFilters;

public class ClaimAuthorizationFilter : IAuthorizationFilter
{
    private readonly IOptions<JWTConfiguration> _jwtOptions;

    public ClaimAuthorizationFilter(IOptions<JWTConfiguration> jwtOptions)
    {
        this._jwtOptions = jwtOptions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var accessToken = context.HttpContext.Request.Headers["accesstoken"];
        var isUnauthorized = true;
        var handler = new JwtSecurityTokenHandler();
        try
        {
            if (!string.IsNullOrEmpty(accessToken) && handler.CanReadToken(accessToken))
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Value.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Value.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = new[] { new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Value.SecretKey)) },
                };
                var claimsPrincipal = handler.ValidateToken(accessToken, validationParameters, out var validatedToken);
                context.HttpContext.User.AddIdentity(new ClaimsIdentity(claimsPrincipal.Claims));
                isUnauthorized = false;
            }

            if (isUnauthorized)
            {
                context.Result = new UnauthorizedResult();
            }
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}