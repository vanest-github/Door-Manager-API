using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Activity.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<ActivityLog>>
{
    private readonly IActivityCommandRepository _activityRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IActivityCommandRepository activityRepository,
        ILogger<IBus> logger)
    {
        this._activityRepository = activityRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<ActivityLog>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        ActivityLog createdLog = default;
        try
        {
            if (request.ActivityLogCreateDto is { })
            {
                var activityLog = new ActivityLog
                {
                    Action = request.ActivityLogCreateDto.Action,
                    ActivityTime = request.ActivityLogCreateDto.ActivityTime,
                    Description = request.ActivityLogCreateDto.Description,
                    UserId = request.ActivityLogCreateDto.UserId,
                    OfficeId = request.ActivityLogCreateDto.OfficeId
                };

                createdLog = await this._activityRepository.CreateAsync(activityLog, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<ActivityLog>.CreateError(ex);
        }
        return ServiceResult<ActivityLog>.CreateSuccess(createdLog);
    }
}
