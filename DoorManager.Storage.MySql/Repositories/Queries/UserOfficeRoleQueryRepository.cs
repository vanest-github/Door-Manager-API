using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class UserOfficeRoleQueryRepository : IUserOfficeRoleQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public UserOfficeRoleQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<IEnumerable<UserOfficeRole>> SearchByPredicateAsync(Expression<Func<UserOfficeRole, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var userOfficeRoles = _dbContext.UserOfficeRoles.Where(predicate);
        return await userOfficeRoles.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}