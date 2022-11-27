using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class RBACQueryRepository : IRBACQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public RBACQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<IEnumerable<RoleFeature>> SearchByPredicateAsync(Expression<Func<RoleFeature, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var roles = _dbContext.RoleFeatures.Where(predicate);
        return await roles.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}