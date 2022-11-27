using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Door.Types;

public class RequestModel : IRequest<ServiceResult<IEnumerable<Entity.DoorType>>>
{
    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door types retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving door types." },
    };
}
