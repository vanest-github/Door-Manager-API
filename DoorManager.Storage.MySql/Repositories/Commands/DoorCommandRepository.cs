using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class DoorCommandRepository : IDoorCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public DoorCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Door> CreateAsync(Door value, CancellationToken cancellationToken = default)
    {
        Door door;
        try
        {
            _ = await _dbContext.Doors.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            door = value;
        }
        catch (Exception)
        {
            throw;
        }

        return door;
    }

    public Task<bool> DeleteAsync(Guid key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Door> Update(Guid key, Door value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Door> Upsert(Guid key, Door value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}