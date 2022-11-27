using System.Collections.Generic;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.UserOfficeRole.Get.ByUserIds;

public class RequestModel : IRequest<ServiceResult<IEnumerable<ActiveUserRoleDto>>>
{
    public RequestModel(int officeId, IEnumerable<long> userIds)
    {
        this.OfficeId = officeId;
        this.UserIds = userIds;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "User access details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving user access details." },
    };

    public int OfficeId { get; set; }

    public IEnumerable<long> UserIds { get; set; }
}
