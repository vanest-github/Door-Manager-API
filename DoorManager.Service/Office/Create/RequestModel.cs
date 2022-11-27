using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Office.Create;

public class RequestModel : IRequest<ServiceResult<Entity.Office>>
{
    public RequestModel(OfficeDto officeDto)
    {
        this.OfficeDto = officeDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Office created successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error creating office." },
    };

    public OfficeDto OfficeDto { get; set; }
}
