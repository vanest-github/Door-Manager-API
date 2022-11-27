using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Role.Get.ById;

public class RequestModel : IRequest<ServiceResult<Entity.Role>>
{
    public RequestModel(int roleId)
    {
        this.RoleId = roleId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Role retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving role details." },
    };

    public int RoleId { get; set; }
}
