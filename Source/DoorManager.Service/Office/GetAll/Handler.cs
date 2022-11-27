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

namespace DoorManager.Service.Office.GetAll;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<Entity.Office>>>
{
    private readonly IOfficeQueryRepository _officeRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IOfficeQueryRepository officeRepository,
        ILogger<IBus> logger)
    {
        this._officeRepository = officeRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<Entity.Office>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<Entity.Office> offices = default;
        try
        {
            var filters = new List<FilterDto>
            {
                new FilterDto { ColumnName = nameof(Entity.Office.IsActive), Values = "true" }
            };
            var officesPredicate = PredicateHelper.CreateFilterPredicate<Entity.Office>(filters);
            offices = await this._officeRepository.SearchByPredicateAsync(officesPredicate, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<Entity.Office>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<Entity.Office>>.CreateSuccess(offices);
    }
}
