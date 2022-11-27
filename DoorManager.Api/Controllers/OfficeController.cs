using DoorManager.Api.Controllers.Base;
using DoorManager.Api.CustomFilters;
using DoorManager.Entity.DTO;
using DoorManager.Service.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace DoorManager.Api.Controllers;

[Route("api/v{v:apiVersion}")]
[ApiVersion("1.0")]
[ApiController]
[ClaimAuthorization]
public class OfficeController : BaseController
{
    private readonly IBus _bus;

    public OfficeController(IBus bus)
    {
        this._bus = bus;
    }

    [HttpPost]
    [Route("office/create")]
    [SwaggerOperation("CreateOffice")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 400, description: "Bad Request")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> CreateOffice([FromBody][Required] OfficeDto officeDto)
    {
        return await this.CreateResponse(
            this._bus.SendAsync(new Service.Office.Create.RequestModel(officeDto)),
            Service.Office.Create.RequestModel.MessageKeys);
    }

    [HttpGet]
    [Route("offices")]
    [SwaggerOperation("GetAllOffices")]
    [SwaggerResponse(statusCode: 200, description: "Success")]
    [SwaggerResponse(statusCode: 500, description: "Server Error")]
    public async Task<IActionResult> GetAllOffices()
    {
        return await this.CreateResponse(this._bus.SendAsync(new Service.Office.GetAll.RequestModel()), Service.Office.GetAll.RequestModel.MessageKeys);
    }
}
