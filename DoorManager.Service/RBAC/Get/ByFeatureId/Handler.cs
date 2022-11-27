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

namespace DoorManager.Service.RBAC.Get.ByFeatureId;

public class Handler : IRequestHandler<RequestModel, ServiceResult<IEnumerable<Entity.RoleFeature>>>
{
    private readonly IRBACQueryRepository _rbacQueryRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IRBACQueryRepository rbacQueryRepository,
        ILogger<IBus> logger)
    {
        this._rbacQueryRepository = rbacQueryRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<Entity.RoleFeature>>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        IEnumerable<Entity.RoleFeature> rbacFeatures;
        try
        {
            var filters = new[]
            {
                new FilterDto { ColumnName = nameof(Entity.RoleFeature.FeatureId), Values = $"{request.FeatureId}" },
                new FilterDto { ColumnName = nameof(Entity.RoleFeature.IsActive), Values = "true" },
            };
            var rbacPredicate = PredicateHelper.CreateFilterPredicate<Entity.RoleFeature>(filters);
            rbacFeatures = await this._rbacQueryRepository.SearchByPredicateAsync(rbacPredicate, cancellationToken).ConfigureAwait(false);
            if (rbacFeatures is not { })
            {
                return ServiceResult<IEnumerable<Entity.RoleFeature>>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<IEnumerable<Entity.RoleFeature>>.CreateError(ex);
        }
        return ServiceResult<IEnumerable<Entity.RoleFeature>>.CreateSuccess(rbacFeatures);
    }
}
