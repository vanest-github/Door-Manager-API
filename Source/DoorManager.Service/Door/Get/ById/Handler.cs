using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using MediatR;
using Microsoft.Extensions.Logging;
using DoorManager.Storage.Interface.Queries;

namespace DoorManager.Service.Door.Get.ById;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Door>>
{
    private readonly IDoorQueryRepository _doorRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IDoorQueryRepository doorRepository,
        ILogger<IBus> logger)
    {
        this._doorRepository = doorRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.Door>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Door door = default;
        try
        {
            door = await this._doorRepository.GetAsync(request.DoorId, cancellationToken).ConfigureAwait(false);
            if (door is not { })
            {
                return ServiceResult<Entity.Door>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Door>.CreateError(ex);
        }
        return ServiceResult<Entity.Door>.CreateSuccess(door);
    }
}
