using DoorManager.Entity;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Commands;

public interface IUserCommandRepository : ICreatable<User>, IUpdatable<long, User>, IDeletable<long>
{
    Task<User> CreateNewUserAsync(User user, UserOfficeRole userRole, CancellationToken cancellationToken = default);
}