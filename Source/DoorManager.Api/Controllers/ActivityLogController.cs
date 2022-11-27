using System.ComponentModel.DataAnnotations;
using DoorManager.Api.Controllers.Base;
using DoorManager.Api.CustomFilters;
using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DoorManager.Api.Controllers;

[Route("api/v{v:apiVersion}")]
[ApiVersion("1.0")]
[ApiController]
[ClaimAuthorization]
public class ActivityLogController : BaseController
{
    private readonly IBus _bus;

    public ActivityLogController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpPost]
    [Route("activityLogs/query")]
    [SwaggerOperation("QueryUserActivites")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> QueryUserActivites([FromBody][Required] ActivityLogQueryDto activityLogQueryDto)
    {
        var isValidFeature = (await this._bus.SendAsync(new Service.RBAC.Validate.RequestModel(User, (int)FeatureType.ActivityLogMonitoring)))?.Data;
        if (isValidFeature != true)
        {
            return await this.CreateResponse(
                Task.FromResult(Service.ServiceResponse.ServiceResult<ActivityLog>.CreateErrorUnauthorized("Permission Denied.")),
                Service.Activity.Query.RequestModel.MessageKeys);
        }

        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Activity.Query.RequestModel(activityLogQueryDto)), Service.Activity.Query.RequestModel.MessageKeys);
    }
}
