using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Door.GetAll;

public class RequestModel : IRequest<ServiceResult<IEnumerable<Entity.Door>>>
{
    public RequestModel(int? officeId)
    {
        this.OfficeId = officeId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving door details." },
    };

    public int? OfficeId { get; set; }
}
