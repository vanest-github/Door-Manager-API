using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class DoorAccessCommandRepository : IDoorAccessCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public DoorAccessCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<DoorAccessRole> CreateAsync(DoorAccessRole value, CancellationToken cancellationToken = default)
    {
        DoorAccessRole doorAccess;
        try
        {
            _ = await _dbContext.DoorAccessRoles.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            doorAccess = value;
        }
        catch (Exception)
        {
            throw;
        }

        return doorAccess;
    }

    public Task<bool> DeleteAsync(long key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DoorAccessRole> Update(long key, DoorAccessRole value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<DoorAccessRole> Upsert(long key, DoorAccessRole value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}