using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Service.Expressions;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Door.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Door>>
{
    private readonly IOfficeQueryRepository _officeRepository;
    private readonly IDoorCommandRepository _doorCommandRepository;
    private readonly IDoorQueryRepository _doorQueryRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IOfficeQueryRepository officeRepository,
        IDoorCommandRepository doorCommandRepository,
        IDoorQueryRepository doorQueryRepository,
        ILogger<IBus> logger)
    {
        this._officeRepository = officeRepository;
        this._doorCommandRepository = doorCommandRepository;
        this._doorQueryRepository = doorQueryRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.Door>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Door createdDoor = default;
        try
        {
            if (request.DoorDto is { })
            {
                var (errorMessage, officeId, doorTypeId) = await ValidateDoorCreation(request.DoorDto, cancellationToken);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return ServiceResult<Entity.Door>.CreateBadRequest(errorMessage);
                }

                var door = new Entity.Door
                {
                    OfficeId = officeId,
                    DoorTypeId = doorTypeId,
                    IsActive = true,
                    CurrentStatus = request.DoorDto.DoorStatus.ToString(),
                    Manufacturer = request.DoorDto.Manufacturer,
                    LockVersion = request.DoorDto.LockVersion,
                    CreatedTime = DateTimeOffset.UtcNow,
                    ModifiedTime = DateTimeOffset.UtcNow
                };
                createdDoor = await this._doorCommandRepository.CreateAsync(door, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Door>.CreateError(ex);
        }
        return ServiceResult<Entity.Door>.Created(createdDoor);
    }

    private async Task<(string, int, int)> ValidateDoorCreation(DoorDto dto, CancellationToken cancellationToken)
    {
        var (errorMessage, officeId, doorTypeId) = (string.Empty, 0, 0);
        var filters = new[]
        {
            new FilterDto { ColumnName = nameof(Entity.Office.OfficeName), Values = $"{dto.OfficeName}" },
            new FilterDto { ColumnName = nameof(Entity.Office.IsActive), Values = "true" }
        };
        var officeNamePredicate = PredicateHelper.CreateFilterPredicate<Entity.Office>(filters);
        var office = (await this._officeRepository.SearchByPredicateAsync(officeNamePredicate, cancellationToken))?.FirstOrDefault();
        if (office is { })
        {
            officeId = office.OfficeId;
            var doorType = await this._doorQueryRepository.GetDoorType(dto.DoorType, cancellationToken);
            if (doorType is { })
            {
                doorTypeId = doorType.DoorTypeId;
            }
            else
            {
                errorMessage = $"Invalid door type: {dto.DoorType}";
            }
        }
        else
        {
            errorMessage = $"Invalid office name: {dto.OfficeName}";
        }
        return (errorMessage, officeId, doorTypeId);
    }
}
