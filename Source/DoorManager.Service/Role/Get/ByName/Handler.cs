using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.DTO;
using DoorManager.Service.Expressions;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.Role.Get.ByName;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.Role>>
{
    private readonly IRoleQueryRepository _roleRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IRoleQueryRepository roleRepository,
        ILogger<IBus> logger)
    {
        this._roleRepository = roleRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.Role>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.Role role;
        try
        {
            var filters = new[]
            {
                new FilterDto { ColumnName = nameof(Entity.Role.RoleName), Values = $"{request.RoleName}" },
                new FilterDto { ColumnName = nameof(Entity.Role.IsActive), Values = "true" }
            };
            var roleByNamePredicate = PredicateHelper.CreateFilterPredicate<Entity.Role>(filters);

            role = (await this._roleRepository.SearchByPredicateAsync(roleByNamePredicate, cancellationToken)).FirstOrDefault();
            if (role is not { })
            {
                return ServiceResult<Entity.Role>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.Role>.CreateError(ex);
        }
        return ServiceResult<Entity.Role>.CreateSuccess(role);
    }
}
