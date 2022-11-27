using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Office.Get;

public class RequestModel : IRequest<ServiceResult<Entity.Office>>
{
    public RequestModel(int officeId)
    {
        this.OfficeId = officeId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Office details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving office details." },
    };

    public int OfficeId { get; set; }
}
