using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.DoorAccess.Delegated;

public class Handler : IRequestHandler<RequestModel, ServiceResult<bool>>
{
    private readonly IDoorAccessQueryRepository _doorAccessRepository;
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IDoorAccessQueryRepository doorAccessRepository,
        IBus bus,
        ILogger<IBus> logger)
    {
        this._doorAccessRepository = doorAccessRepository;
        this._bus = bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<bool>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        bool isValidAccess = false;
        try
        {
            var proxyDoors = await this._doorAccessRepository.GetProxyAccessDoorsAsync(request.UserId, request.OfficeId, cancellationToken).ConfigureAwait(false);
            isValidAccess = proxyDoors.Contains(request.DoorId);

            var activity = ActivityLogType.DelegatedAccess.ToString();
            var activityLogDto = new ActivityLogDto
            {
                Action = activity,
                ActivityTime = DateTimeOffset.UtcNow,
                Description = $"{activity} - {nameof(request.DoorId)} - {request.DoorId} - " + (isValidAccess ? "Success" : "Denied"),
                OfficeId = request.OfficeId,
                UserId = request.UserId
            };
            _ = await this._bus.SendAsync(new Activity.Create.RequestModel(activityLogDto), cancellationToken);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<bool>.CreateError(ex);
        }

        return ServiceResult<bool>.CreateSuccess(isValidAccess);
    }
}
