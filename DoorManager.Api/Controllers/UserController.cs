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
public class UserController : BaseController
{
    private readonly IBus _bus;

    public UserController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpPost]
    [Route("office/{officeId}/user/create")]
    [SwaggerOperation("CreateUser")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> CreateUser([FromRoute][Required] int officeId, [FromBody][Required] UserDto userDto)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.User.Create.RequestModel(officeId, userDto)), Service.User.Create.RequestModel.MessageKeys);
    }

    [HttpGet]
    [Route("user/{userId}")]
    [SwaggerOperation("GetUserDetails")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> GetUserDetails([FromRoute][Required] long userId)
    {
        return await this.CreateResponse(this._bus.SendAsync(new Service.User.Get.RequestModel(userId)), Service.User.Get.RequestModel.MessageKeys);
    }
}
