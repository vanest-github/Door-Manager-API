using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.User.Create;

public class RequestModel : IRequest<ServiceResult<Entity.User>>
{
    public RequestModel(int officeId, UserDto userDto)
    {
        this.OfficeId = officeId;
        this.UserDto = userDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "User created successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error creating user." },
    };

    public int OfficeId { get; set; }

    public UserDto UserDto { get; set; }
}
