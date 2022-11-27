using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Door.Types;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<Entity.DoorType>>>
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

    public async Task<ServiceResult<IEnumerable<Entity.DoorType>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<Entity.DoorType> doorTypes = default;
        try
        {
            var filters = new List<FilterDto>
            {
                new FilterDto { ColumnName = nameof(Entity.DoorType.IsActive), Values = "true" }
            };

            doorTypes = await this._doorRepository.GetDoorTypes(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<Entity.DoorType>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<Entity.DoorType>>.CreateSuccess(doorTypes);
    }
}
