using System;
using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.DoorAccess.Self;

public class RequestModel : IRequest<ServiceResult<bool>>
{
    public RequestModel(Guid doorId, long userId, int officeId)
    {
        this.DoorId = doorId;
        this.UserId = userId;
        this.OfficeId = officeId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door unlocked." },
        { MessageKeyType.ErrorKey.ToString(), "Error unlocking door." },
    };

    public Guid DoorId { get; set; }

    public long UserId { get; set; }

    public int OfficeId { get; set; }
}
