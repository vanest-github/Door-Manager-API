using System.Collections.Generic;
using DoorManager.Entity.Enum;
using DoorManager.Service.ServiceResponse;
using MediatR;

namespace DoorManager.Service.Authentication.GetToken;

public class RequestModel : IRequest<ServiceResult<string>>
{
    public RequestModel(int officeId, long userId)
    {
        this.OfficeId = officeId;
        this.UserId = userId;
    }

    public static Dictionary<string, object> MessageKeys => new()
    {
        { MessageKeyType.SuccessKey.ToString(), "API access token generated successfully." },
        { MessageKeyType.ErrorKey.ToString(), "Error generating API access token." },
    };

    public int OfficeId { get; set; }

    public long UserId { get; set; }
}
