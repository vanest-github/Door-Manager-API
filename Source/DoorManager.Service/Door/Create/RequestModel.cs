using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Door.Create;

public class RequestModel : IRequest<ServiceResult<Entity.Door>>
{
    public RequestModel(DoorDto doorDto)
    {
        this.DoorDto = doorDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Door created successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error creating door." },
    };

    public DoorDto DoorDto { get; set; }
}
