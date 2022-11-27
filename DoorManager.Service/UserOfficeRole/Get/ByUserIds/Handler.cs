using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.UserOfficeRole.Get.ByUserIds;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<ActiveUserRoleDto>>>
{
    private readonly IUserQueryRepository _userRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IUserQueryRepository userRepository,
        ILogger<IBus> logger)
    {
        this._userRepository = userRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ActiveUserRoleDto>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<ActiveUserRoleDto> userRoles = default;
        try
        {
            userRoles = await this._userRepository.GetActiveUserRoles(request.OfficeId, request.UserIds, cancellationToken).ConfigureAwait(false);
            if (userRoles == null || !userRoles.Any())
            {
                return ServiceResult<IEnumerable<ActiveUserRoleDto>>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<ActiveUserRoleDto>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<ActiveUserRoleDto>>.CreateSuccess(userRoles);
    }
}
