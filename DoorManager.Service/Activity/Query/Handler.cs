using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Expressions;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Activity.Query;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<ActivityLog>>>
{
    private readonly IActivityQueryRepository _activityRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IActivityQueryRepository activityRepository,
        ILogger<IBus> logger)
    {
        this._activityRepository = activityRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ActivityLog>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<ActivityLog> activityLogs = default;
        try
        {
            var dto = request.ActivityLogQueryDto;
            if (dto.OfficeId == null && dto.UserId == null && dto.FromTime == null && dto.ToTime == null)
            {
                return ServiceResult<IEnumerable<ActivityLog>>.CreateBadRequest("No query criterion has been set.");
            }

            var filters = new List<FilterDto>();
            if (dto.FromTime.HasValue)
            {
                filters.Add(new FilterDto
                {
                    ColumnName = nameof(ActivityLog.ActivityTime),
                    Operator = FilterOperator.Between,
                    Values = $"{dto.FromTime},{dto.ToTime ?? DateTimeOffset.UtcNow}"
                });
            }
            if (dto.OfficeId.HasValue)
            {
                filters.Add(new FilterDto { ColumnName = nameof(ActivityLog.OfficeId), Values = $"{dto.OfficeId}" });
            }
            if (dto.UserId.HasValue)
            {
                filters.Add(new FilterDto { ColumnName = nameof(ActivityLog.UserId), Values = $"{dto.UserId}" });
            }
            var getByTimePredicate = PredicateHelper.CreateFilterPredicate<ActivityLog>(filters);
            activityLogs = await this._activityRepository.SearchByPredicateAsync(getByTimePredicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<ActivityLog>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<ActivityLog>>.CreateSuccess(activityLogs ?? Enumerable.Empty<ActivityLog>());
    }
}
