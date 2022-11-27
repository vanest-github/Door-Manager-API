using System.ComponentModel.DataAnnotations;
using DoorManager.Api.Controllers.Base;
using DoorManager.Api.CustomFilters;
using DoorManager.Entity.DTO;
using DoorManager.Service.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DoorManager.Api.Controllers;

[Route("api/v{v:apiVersion}")]
[ApiVersion("1.0")]
[ApiController]
[ClaimAuthorization]
public class AccessDelegationController : BaseController
{
    private readonly IBus _bus;

    public AccessDelegationController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpPost]
    [Route("delegate/access")]
    [SwaggerOperation("DelegateDoorAccess")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> DelegateDoorAccess([FromBody][Required] AccessDelegationDto accessDelegationDto)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.UserOfficeRole.Delegate.RequestModel(accessDelegationDto)),
            Service.UserOfficeRole.Delegate.RequestModel.MessageKeys);
    }
}