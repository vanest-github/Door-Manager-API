using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class OfficeQueryRepository : IOfficeQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public OfficeQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Office> GetAsync(int key, CancellationToken cancellationToken = default)
    {
        var office = await _dbContext.Offices.FirstOrDefaultAsync(x => x.OfficeId == key, cancellationToken).ConfigureAwait(false);
        return office!;
    }

    public async Task<IEnumerable<Office>> SearchByPredicateAsync(Expression<Func<Office, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var offices = _dbContext.Offices.Where(predicate);
        return await offices.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}