using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class UserOfficeRoleCommandRepository : IUserOfficeRoleCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public UserOfficeRoleCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<UserOfficeRole> CreateAsync(UserOfficeRole value, CancellationToken cancellationToken = default)
    {
        UserOfficeRole userOfficeRole;
        try
        {
            _ = await _dbContext.UserOfficeRoles.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            userOfficeRole = value;
        }
        catch (Exception)
        {
            throw;
        }

        return userOfficeRole;
    }

    public Task<bool> DeleteAsync(long key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserOfficeRole> Update(long key, UserOfficeRole value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserOfficeRole> Upsert(long key, UserOfficeRole value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}