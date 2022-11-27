using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class RoleCommandRepository : IRoleCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public RoleCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<Role> CreateAsync(Role value, CancellationToken cancellationToken = default)
    {
        Role role;
        try
        {
            _ = await _dbContext.Roles.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            role = value;
        }
        catch (Exception)
        {
            throw;
        }

        return role;
    }

    public Task<bool> DeleteAsync(int key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Role> Update(int key, Role value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Role> Upsert(int key, Role value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}