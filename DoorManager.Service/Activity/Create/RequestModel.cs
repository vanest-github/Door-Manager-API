using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Activity.Create;

public class RequestModel : IRequest<ServiceResult<ActivityLog>>
{
    public RequestModel(ActivityLogDto activityLogDto)
    {
        this.ActivityLogCreateDto = activityLogDto;
    }

    public ActivityLogDto ActivityLogCreateDto { get; set; }
}
