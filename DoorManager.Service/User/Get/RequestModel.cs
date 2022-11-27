using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.User.Get;

public class RequestModel : IRequest<ServiceResult<Entity.User>>
{
    public RequestModel(long userId)
    {
        this.UserId = userId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "User details retrieved successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error retrieving user details." },
    };

    public long UserId { get; set; }
}
