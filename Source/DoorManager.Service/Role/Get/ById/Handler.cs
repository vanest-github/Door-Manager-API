using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Role.Get.ById;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Role>>
{
    private readonly IRoleQueryRepository _roleRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IRoleQueryRepository roleRepository,
        ILogger<IBus> logger)
    {
        this._roleRepository = roleRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.Role>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Role role;
        try
        {
            role = await this._roleRepository.GetAsync(request.RoleId, cancellationToken).ConfigureAwait(false);
            if (role is not { })
            {
                return ServiceResult<Entity.Role>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Role>.CreateError(ex);
        }
        return ServiceResult<Entity.Role>.CreateSuccess(role);
    }
}
