using System;
using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.DoorAccess.Unlock;

public class RequestModel : IRequest<ServiceResult<bool>>
{
    public RequestModel(DoorUnlockDto doorUnlockDto)
    {
        this.DoorUnlockDto = doorUnlockDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door unlocked." },
        { MessageKeyType.ErrorKey.ToString(), "Error unlocking door." },
    };

    public DoorUnlockDto DoorUnlockDto { get; set; }
}
