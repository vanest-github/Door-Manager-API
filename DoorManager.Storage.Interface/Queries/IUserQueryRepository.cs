using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Storage.Interface.Base;

namespace DoorManager.Storage.Interface.Queries;

public interface IUserQueryRepository : IReadable<long, User>, ISearchable<User>
{
    Task<IEnumerable<ActiveUserRoleDto>> GetActiveUserRoles(int officeId, IEnumerable<long> userIds, CancellationToken cancellationToken = default);
}