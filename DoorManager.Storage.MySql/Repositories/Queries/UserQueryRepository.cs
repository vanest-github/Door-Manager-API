using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Entity.DTO;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public UserQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<User> GetAsync(long key, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == key, cancellationToken).ConfigureAwait(false);
        return user!;
    }

    public async Task<IEnumerable<ActiveUserRoleDto>> GetActiveUserRoles(int officeId, IEnumerable<long> userIds, CancellationToken cancellationToken)
    {
        var userIdSet = userIds.ToHashSet();
        var userOfficeRoleQuery = from uoRole in _dbContext.UserOfficeRoles
                                  join user in _dbContext.Users on uoRole.UserId equals user.UserId
                                  join role in _dbContext.Roles on uoRole.RoleId equals role.RoleId
                                  where user.IsActive == true && role.IsActive == true
                                  && uoRole.ValidFrom <= DateTimeOffset.UtcNow && uoRole.ValidTo >= DateTimeOffset.UtcNow
                                  && uoRole.OfficeId == officeId
                                  && userIdSet.Contains(user.UserId)
                                  select new ActiveUserRoleDto
                                  {
                                      OfficeId = uoRole.OfficeId,
                                      UserId = uoRole.UserId,
                                      RoleId = uoRole.RoleId,
                                      RoleName = role.RoleName,
                                      RolePriority = role.RolePriority,
                                      OnBehalfUserId = uoRole.OnBehalfUserId,
                                      ValidFrom = uoRole.ValidFrom,
                                      ValidTo = uoRole.ValidTo
                                  };
        return await userOfficeRoleQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<User>> SearchByPredicateAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var users = _dbContext.Users.Where(predicate);
        return await users.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}