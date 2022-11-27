using DoorManager.Service.ServiceResponse;
using MediatR;
using System.Security.Claims;

namespace DoorManager.Service.RBAC.Validate;

public class RequestModel : IRequest<ServiceResult<bool>>
{
    public RequestModel(ClaimsPrincipal claimsPrincipal, int featureId)
    {
        this.ClaimsPrincipal = claimsPrincipal;
        this.FeatureId = featureId;
    }

    public ClaimsPrincipal ClaimsPrincipal { get; set; }

    public int FeatureId { get; set; }
}
