using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;
using System.Collections.Generic;

namespace DoorManager.Service.UserOfficeRole.Create;

public class RequestModel : IRequest<ServiceResult<Entity.UserOfficeRole>>
{
    public RequestModel(UserOfficeRoleDto userOfficeRoleDto)
    {
        this.UserOfficeRoleDto = userOfficeRoleDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "User access created successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error creating user access." },
    };

    public UserOfficeRoleDto UserOfficeRoleDto { get; set; }
}
