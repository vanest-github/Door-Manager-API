using System.Linq.Expressions;
using DoorManager.Entity;
using DoorManager.Storage.Interface.Queries;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql.Repositories.Queries;

public class DoorQueryRepository : IDoorQueryRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public DoorQueryRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Door> GetAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var door = await _dbContext.Doors.FirstOrDefaultAsync(x => x.DoorId == key, cancellationToken).ConfigureAwait(false);
        return door!;
    }

    public async Task<DoorType> GetDoorType(string doorTypeName, CancellationToken cancellationToken = default)
    {
        var doorType = await _dbContext.DoorTypes.FirstOrDefaultAsync(x => x.DoorTypeName == doorTypeName, cancellationToken).ConfigureAwait(false);
        return doorType!;
    }

    public async Task<IEnumerable<DoorType>> GetDoorTypes(CancellationToken cancellationToken = default)
    {
        var doorTypes = _dbContext.DoorTypes.Where(x => x.IsActive);
        return await doorTypes.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Door>> SearchByPredicateAsync(Expression<Func<Door, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var doors = _dbContext.Doors.Where(predicate);
        return await doors.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}