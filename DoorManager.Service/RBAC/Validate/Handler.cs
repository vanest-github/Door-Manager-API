using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.RBAC.Validate;

public class Handler : IRequestHandler<RequestModel, ServiceResult<bool>>
{
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IBus bus,
        ILogger<IBus> logger)
    {
        this._bus = bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<bool>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        var isAuthorizedFeature = false;
        try
        {
            var roleFeatures = (await this._bus.SendAsync(new Get.ByFeatureId.RequestModel(request.FeatureId), cancellationToken))?.Data;
            if (roleFeatures is { })
            {
                var roleId = Convert.ToInt32(request.ClaimsPrincipal.FindFirst(ClaimTypes.Role)!.Value);
                isAuthorizedFeature = roleFeatures.Any(x => x.RoleId == roleId);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<bool>.CreateError(ex);
        }
        return ServiceResult<bool>.CreateSuccess(isAuthorizedFeature);
    }
}
