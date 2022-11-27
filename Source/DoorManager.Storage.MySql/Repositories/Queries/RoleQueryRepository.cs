using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class RoleQueryRepository : IRoleQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public RoleQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Role> GetAsync(int key, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == key, cancellationToken).ConfigureAwait(false);
        return role!;
    }

    public async Task<IEnumerable<Role>> SearchByPredicateAsync(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var roles = _dbContext.Roles.Where(predicate);
        return await roles.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}