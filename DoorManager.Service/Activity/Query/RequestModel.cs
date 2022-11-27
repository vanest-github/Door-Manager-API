using System.Collections.Generic;
using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Activity.Query;

public class RequestModel : IRequest<ServiceResult<IEnumerable<ActivityLog>>>
{
    public RequestModel(ActivityLogQueryDto activityLogQueryDto)
    {
        this.ActivityLogQueryDto = activityLogQueryDto;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "Activity logs retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving activity logs." },
    };

    public ActivityLogQueryDto ActivityLogQueryDto { get; set; }
}
