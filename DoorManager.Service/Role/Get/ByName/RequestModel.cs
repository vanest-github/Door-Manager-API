using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Role.Get.ByName;

public class RequestModel : IRequest<ServiceResult<Entity.Role>>
{
    public RequestModel(string roleName)
    {
        this.RoleName = roleName;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Role retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving role details." },
    };

    public string RoleName { get; set; }
}
