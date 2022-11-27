using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class OfficeCommandRepository : IOfficeCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public OfficeCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Office> CreateAsync(Office value, CancellationToken cancellationToken = default)
    {
        Office office;
        try
        {
            _ = await _dbContext.Offices.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            office = value;
        }
        catch (Exception)
        {
            throw;
        }

        return office;
    }

    public Task<bool> DeleteAsync(Office key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Office> Update(int key, Office value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Office> Upsert(int key, Office value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}