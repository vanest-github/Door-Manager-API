using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class DoorAccessQueryRepository : IDoorAccessQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public DoorAccessQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<IEnumerable<Guid>> GetSelfAccessDoorsAsync(long userId, int officeId, CancellationToken cancellationToken)
    {
        IEnumerable<Guid>? doorIds = default;
        try
        {
            var doorQuery = from user in _dbContext.Users
                            join uRole in _dbContext.UserOfficeRoles on user.UserId equals uRole.UserId
                            join role in _dbContext.Roles on uRole.RoleId equals role.RoleId
                            join dAccess in _dbContext.DoorAccessRoles on role.RoleId equals dAccess.RoleId
                            join door in _dbContext.Doors on dAccess.DoorTypeId equals door.DoorTypeId
                            where user.IsActive && role.IsActive && door.IsActive && uRole.OnBehalfUserId == null
                            && uRole.ValidFrom <= DateTimeOffset.UtcNow && uRole.ValidTo >= DateTimeOffset.UtcNow
                            && dAccess.AccessFrom <= DateTimeOffset.UtcNow && dAccess.AccessTo >= DateTimeOffset.UtcNow
                            && user.UserId == userId
                            && dAccess.OfficeId == officeId
                            select door.DoorId;
            doorIds = await doorQuery.ToListAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }

        return doorIds ?? Enumerable.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> GetProxyAccessDoorsAsync(long userId, int officeId, CancellationToken cancellationToken)
    {
        IEnumerable<Guid>? doorIds = default;
        try
        {
            var proxyDoorQuery = from user in _dbContext.Users
                                 join uoRole in _dbContext.UserOfficeRoles on user.UserId equals uoRole.UserId
                                 join prRole in _dbContext.UserOfficeRoles on uoRole.OnBehalfUserId equals prRole.UserId
                                 join prUsr in _dbContext.Users on prRole.UserId equals prUsr.UserId
                                 join role in _dbContext.Roles on uoRole.RoleId equals role.RoleId
                                 join dAccess in _dbContext.DoorAccessRoles on role.RoleId equals dAccess.RoleId
                                 join door in _dbContext.Doors on dAccess.DoorTypeId equals door.DoorTypeId
                                 where door.IsActive && user.IsActive && prUsr.IsActive && role.IsActive && uoRole.OnBehalfUserId != null
                                 && uoRole.ValidFrom <= DateTimeOffset.UtcNow && uoRole.ValidTo >= DateTimeOffset.UtcNow
                                 && prRole.ValidFrom <= DateTimeOffset.UtcNow && prRole.ValidTo >= DateTimeOffset.UtcNow
                                 && dAccess.AccessFrom <= DateTimeOffset.UtcNow && dAccess.AccessTo >= DateTimeOffset.UtcNow
                                 && user.UserId == userId
                                 && dAccess.OfficeId == officeId
                                 select door.DoorId;
            doorIds = await proxyDoorQuery.ToListAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }

        return doorIds ?? Enumerable.Empty<Guid>();
    }

    public async Task<IEnumerable<DoorAccessRole>> SearchByPredicateAsync(Expression<Func<DoorAccessRole, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var doorAccessRoles = _dbContext.DoorAccessRoles.Where(predicate);
        return await doorAccessRoles.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}