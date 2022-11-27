using System.ComponentModel.DataAnnotations;
using DoorManager.Api.Controllers.Base;
using DoorManager.Service.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoorManager.Api.Controllers;

[Route("api/v{v:apiVersion}")]
[ApiVersion("1.0")]
[ApiController]
[ApiExplorerSettings(GroupName = "DoorManager.Authentication.Api")]
public class AuthenticationController : BaseController
{
    private readonly IBus _bus;

    public AuthenticationController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpGet]
    [Route("office/{officeId}/user/{userId}")]
    public async Task<IActionResult> GetToken([FromRoute][Required] int officeId, [FromRoute][Required] long userId)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Authentication.GetToken.RequestModel(officeId, userId)),
            Service.Authentication.GetToken.RequestModel.MessageKeys);
    }
}