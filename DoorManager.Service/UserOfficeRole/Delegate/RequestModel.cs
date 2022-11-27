using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.UserOfficeRole.Delegate;

public class RequestModel : IRequest<ServiceResult<Entity.UserOfficeRole>>
{
    public RequestModel(AccessDelegationDto accessDelegationDto)
    {
        this.AccessDelegationDto = accessDelegationDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door access delegated successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error delegating door access." },
    };

    public AccessDelegationDto AccessDelegationDto { get; set; }
}
