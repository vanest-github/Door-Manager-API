using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.Configurations;
using DoorManager.Entity.DTO;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DoorManager.Service.Authentication.GetToken;

public class Handler : IRequestHandler<RequestModel, ServiceResult<string>>
{
    private readonly JWTConfiguration _options;
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
      IOptions<JWTConfiguration> optionsAccessor,
      IBus bus,
      ILogger<IBus> logger)
    {
        this._options = optionsAccessor.Value;
        this._bus = bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<string>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        string accessToken = default;
        try
        {
            var userOfficeRoles = (await this._bus.SendAsync(new UserOfficeRole.Get.ByUserIds.RequestModel(request.OfficeId, new[] { request.UserId })))?.Data;
            var userDefaultRole = userOfficeRoles?.FirstOrDefault(x => !x.OnBehalfUserId.HasValue);
            if (userDefaultRole is not { })
            {
                return ServiceResult<string>.CreateErrorResourceNotFound();
            }

            accessToken = CreateJwtToken(userDefaultRole);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<string>.CreateError(ex);
        }
        return ServiceResult<string>.CreateSuccess(accessToken);
    }

    public string CreateJwtToken(ActiveUserRoleDto userDefaultRole)
    {
        var key = Encoding.ASCII.GetBytes(_options.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, $"{userDefaultRole.UserId}"),
                new Claim(ClaimTypes.Role, $"{userDefaultRole.RoleId}")
            }),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_options.TokenExpirationDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    private string GetToken(long userId, string userName)
    {
        var payload = new Dictionary<string, object>
        {
            { "id", userId },
            { "name", userName},
            { "iss", _options.Issuer },
            { "aud", _options.Audience },
        };

        var currentUnixTs = GetUnixTsSeconds(DateTime.UtcNow);
        var tokenExpirationTs = GetUnixTsSeconds(DateTime.UtcNow.AddDays(_options.TokenExpirationDays));
        payload.Add("nbf", currentUnixTs);
        payload.Add("iat", currentUnixTs);
        payload.Add("exp", tokenExpirationTs);

        var encoder = new JwtEncoder(new HMACSHA512Algorithm(), new JsonNetSerializer(), new JwtBase64UrlEncoder());

        return encoder.Encode(payload, _options.SecretKey);
    }

    private static int GetUnixTsSeconds(DateTime dateTime)
    {
        return (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
