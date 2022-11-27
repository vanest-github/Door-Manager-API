using System.Collections.Generic;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.RBAC.Get.ByFeatureId;

public class RequestModel : IRequest<ServiceResult<IEnumerable<Entity.RoleFeature>>>
{
    public RequestModel(int featureId)
    {
        this.FeatureId = featureId;
    }

    public int FeatureId { get; set; }
}
