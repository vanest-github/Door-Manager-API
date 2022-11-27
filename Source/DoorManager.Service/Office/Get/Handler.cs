using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Office.Get;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Office>>
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

    public async Task<ServiceResult<Entity.Office>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Office office = default;
        try
        {
            office = await this._officeRepository.GetAsync(request.OfficeId, cancellationToken).ConfigureAwait(false);
            if (office is not { })
            {
                return ServiceResult<Entity.Office>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Office>.CreateError(ex);
        }
        return ServiceResult<Entity.Office>.CreateSuccess(office);
    }
}
