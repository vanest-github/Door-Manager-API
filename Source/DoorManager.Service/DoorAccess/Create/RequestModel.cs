using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.DoorAccess.Create;

public class RequestModel : IRequest<ServiceResult<Entity.DoorAccessRole>>
{
    public RequestModel(AccessProvisionDto accessProvisionDto)
    {
        this.AccessProvisionDto = accessProvisionDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Access granted successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error granting door access to the role." },
    };

    public AccessProvisionDto AccessProvisionDto { get; set; }
}
