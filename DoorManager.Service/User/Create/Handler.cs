using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoorManager.Entity.Configurations;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Expressions;
using DoorManager.Service.Infrastructure.Interfaces;
using DoorManager.Service.ServiceResponse;
using DoorManager.Storage.Interface.Commands;
using DoorManager.Storage.Interface.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DoorManager.Service.User.Create;

public class Handler : IRequestHandler<RequestModel, ServiceResult<Entity.User>>
{
    private readonly IUserCommandRepository _userCommandRepository;
    private readonly IUserQueryRepository _userQueryRepository;
    private readonly IOptions<GlobalConfiguration> _configuration;
    private readonly IBus _bus;
    private readonly ILogger<IBus> _logger;

    public Handler(
        IUserCommandRepository userCommandRepository,
        IUserQueryRepository userQueryRepository,
        IOptions<GlobalConfiguration> configuration,
        IBus _bus,
        ILogger<IBus> logger)
    {
        this._userCommandRepository = userCommandRepository;
        this._userQueryRepository = userQueryRepository;
        this._configuration = configuration;
        this._bus = _bus;
        this._logger = logger;
    }

    public async Task<ServiceResult<Entity.User>> Handle(RequestModel request, CancellationToken cancellationToken)
    {
        Entity.User newUser;
        try
        {
            var dto = request.UserDto;
            var (errorMessage, roleId) = await ValidateUserCreation(dto, cancellationToken);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return ServiceResult<Entity.User>.CreateBadRequest(errorMessage);
            }

            var user = new Entity.User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IsActive = dto.IsActive
            };
            var userOfficeRole = new Entity.UserOfficeRole
            {
                OfficeId = request.OfficeId,
                RoleId = roleId,
                ValidFrom = DateTimeOffset.UtcNow,
                ValidTo = DateTimeOffset.UtcNow.AddMonths(_configuration.Value.UserRoleValidityMonths)
            };

            newUser = await this._userCommandRepository.CreateNewUserAsync(user, userOfficeRole, cancellationToken).ConfigureAwait(false);
            if (newUser is { })
            {
                var activity = ActivityLogType.CreateUser.ToString();
                var activityLogDto = new ActivityLogDto
                {
                    Action = activity,
                    ActivityTime = DateTimeOffset.UtcNow,
                    Description = $"{activity} - {nameof(newUser.UserId)} - {newUser.UserId} - Success",
                    OfficeId = request.OfficeId,
                    UserId = dto.LastModifiedBy
                };
                _ = await this._bus.SendAsync(new Activity.Create.RequestModel(activityLogDto), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"{ex.Message + ex.StackTrace} in {typeof(Handler).FullName}.{nameof(this.Handle)}");
            return ServiceResult<Entity.User>.CreateError(ex);
        }
        return ServiceResult<Entity.User>.Created(newUser);
    }

    private async Task<(string, int)> ValidateUserCreation(UserDto userDto, CancellationToken cancellationToken)
    {
        var (errorMessage, roleId) = (string.Empty, 0);
        try
        {
            var creatingUser = (await this._bus.SendAsync(new Get.RequestModel(userDto.LastModifiedBy), cancellationToken))?.Data;
            if (creatingUser is { })
            {
                var newUserRole = (await this._bus.SendAsync(new Role.Get.ByName.RequestModel(userDto.RoleName), cancellationToken))?.Data;
                if (newUserRole is { })
                {
                    roleId = newUserRole.RoleId;
                    var filters = new[]
                    {
                        new FilterDto { ColumnName = nameof(Entity.User.FirstName), Values = userDto.FirstName },
                        new FilterDto { ColumnName = nameof(Entity.User.LastName), Values = userDto.LastName },
                    };
                    var userNamePredicate = PredicateHelper.CreateFilterPredicate<Entity.User>(filters);
                    var userWithSameName = (await this._userQueryRepository.SearchByPredicateAsync(userNamePredicate))?.FirstOrDefault();
                    if (userWithSameName is { })
                    {
                        errorMessage = "First and last name combo exists.";
                    }
                }
                else
                {
                    errorMessage = "Invalid user role.";
                }
            }
            else
            {
                errorMessage = "Invalid modifying user id.";
            }
        }
        catch (Exception)
        {
            throw;
        }

        return (errorMessage, roleId);
    }
}
