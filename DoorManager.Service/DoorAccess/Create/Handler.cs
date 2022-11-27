using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.DoorAccess.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.DoorAccessRole>>
{
    private readonly IDoorQueryRepository _doorRepository;
    private readonly IDoorAccessCommandRepository _doorAccessRepository;
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IDoorQueryRepository doorRepository,
        IDoorAccessCommandRepository doorAccessRepository,
        IBus _bus,
        ILogger<IBus> logger)
    {
        this._doorRepository = doorRepository;
        this._doorAccessRepository = doorAccessRepository;
        this._bus = _bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.DoorAccessRole>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.DoorAccessRole newAccessProvision;
        try
        {
            var dto = request.AccessProvisionDto;
            var (errorMessage, doorTypeId, roleId) = await ValidateDoorAccess(dto, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return ServiceResult<Entity.DoorAccessRole>.CreateBadRequest(errorMessage);
            }

            var doorAccess = new Entity.DoorAccessRole
            {
                OfficeId = dto.OfficeId,
                DoorTypeId = doorTypeId,
                RoleId = roleId,
                AccessFrom = dto.AccessFrom ?? DateTimeOffset.UtcNow,
                AccessTo = dto.AccessTo,
            };
            newAccessProvision = await this._doorAccessRepository.CreateAsync(doorAccess, cancellationToken).ConfigureAwait(false);
            if (newAccessProvision is { })
            {
                var activity = ActivityLogType.AccessProvision.ToString();
                var activityLogDto = new ActivityLogDto
                {
                    Action = activity,
                    ActivityTime = DateTimeOffset.UtcNow,
                    Description = $"{activity} - {nameof(newAccessProvision.DoorAccessId)} - {newAccessProvision.DoorAccessId} - {dto.DoorType} - {dto.RoleName} - Success",
                    OfficeId = dto.OfficeId,
                    UserId = 1
                };
                _ = await this._bus.SendAsync(new Activity.Create.RequestModel(activityLogDto), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.DoorAccessRole>.CreateError(ex);
        }
        return ServiceResult<Entity.DoorAccessRole>.CreateSuccess(newAccessProvision);
    }

    private async Task<(string, int, int)> ValidateDoorAccess(AccessProvisionDto accessProvisionDto, CancellationToken cancellationToken)
    {
        var (errorMessage, doorTypeId, roleId) = (string.Empty, 0, 0);
        if (new[] { accessProvisionDto.AccessFrom, DateTimeOffset.UtcNow }.Max() >= accessProvisionDto.AccessTo)
        {
            errorMessage = "Invalid date range.";
        }
        else
        {
            var doorType = await this._doorRepository.GetDoorType(accessProvisionDto.DoorType, cancellationToken).ConfigureAwait(false);
            if (doorType is { })
            {
                doorTypeId = doorType.DoorTypeId;
                var role = (await this._bus.SendAsync(new Role.Get.ByName.RequestModel(accessProvisionDto.RoleName), cancellationToken))?.Data;
                if (role is { })
                {
                    roleId = role.RoleId;
                }
                else
                {
                    errorMessage = "Invalid role name.";
                }
            }
            else
            {
                errorMessage = "Invalid door type.";
            }
        }

        return (errorMessage, doorTypeId, roleId);
    }
}
