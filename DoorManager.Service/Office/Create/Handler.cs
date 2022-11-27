using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Office.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Office>>
{
    private readonly IOfficeCommandRepository _officeRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IOfficeCommandRepository officeRepository,
        ILogger<IBus> logger)
    {
        this._officeRepository = officeRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.Office>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Office office = default;
        try
        {
            if (request.OfficeDto is { })
            {
                office = new Entity.Office
                {
                    OfficeName = request.OfficeDto.OfficeName,
                    Country = request.OfficeDto.Country,
                    IsActive = request.OfficeDto.IsActive,
                    Latitude = request.OfficeDto.Latitude,
                    Longitude = request.OfficeDto.Longitude
                };

                office = await this._officeRepository.CreateAsync(office, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Office>.CreateError(ex);
        }
        return ServiceResult<Entity.Office>.Created(office);
    }
}
