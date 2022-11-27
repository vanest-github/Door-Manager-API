using System;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DoorManager.Service.User.Get;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.User>>
{
    private readonly IUserQueryRepository _userRepository;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IUserQueryRepository userRepository,
        ILogger<IBus> logger)
    {
        this._userRepository = userRepository;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.User>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.User user;
        try
        {
            user = await this._userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            if (user is not { })
            {
                return ServiceResult<Entity.User>.CreateErrorResourceNotFound();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.User>.CreateError(ex);
        }
        return ServiceResult<Entity.User>.CreateSuccess(user);
    }
}
