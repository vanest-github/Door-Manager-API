using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Service.Expressions;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Door.GetAll;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<Entity.Door>>>
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

    public async Task<ServiceResult<IEnumerable<Entity.Door>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<Entity.Door> doors = default;
        try
        {
            var filters = new List<FilterDto>
            {
                new FilterDto { ColumnName = nameof(Entity.Door.IsActive), Values = "true" }
            };
            if (request.OfficeId.HasValue)
            {
                filters.Add(new FilterDto { ColumnName = nameof(Entity.Door.OfficeId), Values = request.OfficeId.Value.ToString() });
            }

            var doorsPredicate = PredicateHelper.CreateFilterPredicate<Entity.Door>(filters);
            doors = await this._doorRepository.SearchByPredicateAsync(doorsPredicate, cancellationToken).ConfigureAwait(false);
            if (doors is not { })
            {
                return ServiceResult<IEnumerable<Entity.Door>>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<Entity.Door>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<Entity.Door>>.CreateSuccess(doors);
    }
}
