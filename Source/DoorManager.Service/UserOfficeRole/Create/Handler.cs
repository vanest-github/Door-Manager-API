using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.UserOfficeRole.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.UserOfficeRole>>
{
    private readonly IUserOfficeRoleCommandRepository _userOfficeRoleRepository;
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IUserOfficeRoleCommandRepository userOfficeRoleRepository,
        IBus _bus,
        ILogger<IBus> logger)
    {
        this._userOfficeRoleRepository = userOfficeRoleRepository;
        this._bus = _bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.UserOfficeRole>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.UserOfficeRole addedUserRole;
        try
        {
            var userOfficeRole = new Entity.UserOfficeRole
            {
                OfficeId = request.UserOfficeRoleDto.OfficeId,
                OnBehalfUserId = request.UserOfficeRoleDto.OnBehalfUserId,
                RoleId = request.UserOfficeRoleDto.RoleId,
                UserId = request.UserOfficeRoleDto.UserId,
                ValidFrom = request.UserOfficeRoleDto.ValidFrom ?? DateTimeOffset.UtcNow,
                ValidTo = request.UserOfficeRoleDto.ValidTo
            };
            addedUserRole = await this._userOfficeRoleRepository.CreateAsync(userOfficeRole, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.UserOfficeRole>.CreateError(ex);
        }
        return ServiceResult<Entity.UserOfficeRole>.CreateSuccess(addedUserRole);
    }
}
