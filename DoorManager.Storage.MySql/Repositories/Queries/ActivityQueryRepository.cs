using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class ActivityQueryRepository : IActivityQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public ActivityQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<IEnumerable<ActivityLog>> SearchByPredicateAsync(Expression<Func<ActivityLog, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var activityLogs = _dbContext.ActivityLogs.Where(predicate);
        return await activityLogs.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}