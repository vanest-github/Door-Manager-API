using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.Enum;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.DoorAccess.Unlock;

public class Handler : IRequestHandler<RequestModel, ServiceResult<bool>>
{
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IBus bus,
        ILogger<IBus> logger)
    {
        this._bus = bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<bool>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        bool isValidAccess = false;
        try
        {
            var dto = request.DoorUnlockDto;
            if (dto is { })
            {
                ServiceResult<bool> doorAccessResult;
                if (dto.DoorAccessMode == DoorAccessMode.SelfAccess)
                {
                    doorAccessResult = await this._bus.SendAsync(new Self.RequestModel(dto.DoorId, dto.UserId, dto.OfficeId), cancellationToken);
                }
                else
                {
                    doorAccessResult = await this._bus.SendAsync(new Delegated.RequestModel(dto.DoorId, dto.UserId, dto.OfficeId), cancellationToken);
                }

                isValidAccess = doorAccessResult?.Data ?? false;
                if (!isValidAccess)
                {
                    return ServiceResult<bool>.CreateErrorUnauthorized("Access Denied.");
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<bool>.CreateError(ex);
        }

        return ServiceResult<bool>.CreateSuccess(isValidAccess);
    }
}
