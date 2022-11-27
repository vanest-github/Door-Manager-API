using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Office.GetAll;

public class RequestModel : IRequest<ServiceResult<IEnumerable<Entity.Office>>>
{
    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Office details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving office details." },
    };
}
