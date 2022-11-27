using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.UserOfficeRole.Delegate;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.UserOfficeRole>>
{
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IBus _bus,
        ILogger<IBus> logger)
    {
        this._bus = _bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.UserOfficeRole>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.UserOfficeRole delegatedRole;
        try
        {
            var (isValid, errorMessage) = await ValidateDelegation(request.AccessDelegationDto, cancellationToken);
            if (!isValid)
            {
                return ServiceResult<Entity.UserOfficeRole>.CreateBadRequest(errorMessage);
            }

            var userOfficeRoleDto = new UserOfficeRoleDto
            {
                OfficeId = request.AccessDelegationDto.OfficeId,
                UserId = request.AccessDelegationDto.TargetUserId,
                OnBehalfUserId = request.AccessDelegationDto.IssuingUserId,
                RoleId = request.AccessDelegationDto.RoleId,
                ValidFrom = request.AccessDelegationDto.ValidFrom ?? DateTimeOffset.UtcNow,
                ValidTo = request.AccessDelegationDto.ValidTo
            };
            delegatedRole = (await this._bus.SendAsync(new Create.RequestModel(userOfficeRoleDto), cancellationToken).ConfigureAwait(false))?.Data;

            if (delegatedRole is { })
            {
                var activity = ActivityLogType.AccessDelegation.ToString();
                var activityLogDto = new ActivityLogDto
                {
                    OfficeId = request.AccessDelegationDto.OfficeId,
                    UserId = request.AccessDelegationDto.IssuingUserId,
                    Action = activity,
                    ActivityTime = DateTimeOffset.UtcNow,
                    Description = $"{activity} - {nameof(delegatedRole.UserOfficeRoleId)} - {delegatedRole.UserOfficeRoleId} - Success",
                };
                _ = await this._bus.SendAsync(new Activity.Create.RequestModel(activityLogDto), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.UserOfficeRole>.CreateError(ex);
        }
        return ServiceResult<Entity.UserOfficeRole>.CreateSuccess(delegatedRole);
    }

    private async Task<(bool, string)> ValidateDelegation(AccessDelegationDto dto, CancellationToken cancellationToken)
    {
        var (isValid, errorMessage) = (false, string.Empty);
        try
        {
            if (new[] { dto.ValidFrom, DateTimeOffset.UtcNow }.Max() >= dto.ValidTo)
            {
                errorMessage = "Invalid date range.";
            }
            else
            {
                var userRoles = (await this._bus.SendAsync(new Get.ByUserIds.RequestModel(dto.OfficeId, new[] { dto.IssuingUserId, dto.TargetUserId }), cancellationToken))?.Data;
                var issuingUserRoles = userRoles?.Where(x => x.UserId == dto.IssuingUserId);
                var targetUserRoles = userRoles?.Where(x => x.UserId == dto.TargetUserId);
                if (issuingUserRoles is not { } || !issuingUserRoles.Any())
                {
                    errorMessage = "Invalid or inactive user.";
                }
                else if (targetUserRoles is not { } || !targetUserRoles.Any())
                {
                    errorMessage = "Invalid or inactive target user.";
                }
                else if (targetUserRoles.FirstOrDefault(x => x.RoleId == dto.RoleId) is { })
                {
                    errorMessage = "Target user already has the role.";
                }
                else if (!await IsValidPriorityRole(issuingUserRoles, dto.RoleId, cancellationToken))
                {
                    errorMessage = "Invalid role to delegate.";
                }
                else
                {
                    isValid = true;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        return (isValid, errorMessage);
    }

    private async Task<bool> IsValidPriorityRole(IEnumerable<ActiveUserRoleDto> fromRoles, int toRoleId, CancellationToken cancellationToken)
    {
        if (fromRoles.Any(x => x.RoleId == toRoleId))
        {
            return true;
        }

        var toRole = (await this._bus.SendAsync(new Role.Get.ById.RequestModel(toRoleId), cancellationToken))?.Data;

        return toRole is { } && toRole.IsActive && fromRoles.Any(x => x.RolePriority <= toRole.RolePriority);
    }
}
