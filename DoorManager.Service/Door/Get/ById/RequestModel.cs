using System;
using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Door.Get.ById;

public class RequestModel : IRequest<ServiceResult<Entity.Door>>
{
    public RequestModel(Guid doorId, int officeId)
    {
        this.DoorId = doorId;
        this.OfficeId = officeId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving door details." },
    };

    public Guid DoorId { get; set; }

    public int OfficeId { get; set; }
}
