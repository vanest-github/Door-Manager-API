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
public class DoorController : BaseController
{
    private readonly IBus _bus;

    public DoorController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpPost]
    [Route("door/create")]
    [SwaggerOperation("CreateDoor")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> CreateDoor([FromBody][Required] DoorDto doorDto)
    {
        return await this.CreateResponse(this._bus.SendAsync(new Service.Door.Create.RequestModel(doorDto)), Service.Door.Create.RequestModel.MessageKeys);
    }

    [HttpGet]
    [Route("office/{officeId}/door/{doorId}")]
    [SwaggerOperation("GetDoorDetails")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> GetDoorDetails([FromRoute][Required] int officeId, [FromRoute][Required] Guid doorId)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Door.Get.ById.RequestModel(doorId, officeId)),
            Service.Door.Get.ById.RequestModel.MessageKeys);
    }

    [HttpGet]
    [Route("office/{officeId}/doors")]
    [SwaggerOperation("GetAllDoors")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> GetAllDoors([FromRoute][Required] int officeId)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Door.GetAll.RequestModel(officeId)),
            Service.Door.GetAll.RequestModel.MessageKeys);
    }

    [HttpGet]
    [Route("doorTypes")]
    [SwaggerOperation("GetDoorTypes")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> GetDoorTypes()
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Door.Types.RequestModel()),
            Service.Door.Types.RequestModel.MessageKeys);
    }
}
