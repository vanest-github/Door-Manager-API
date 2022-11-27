using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class UserCommandRepository : IUserCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public UserCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<User> CreateNewUserAsync(User user, UserOfficeRole userRole, CancellationToken cancellationToken = default)
    {
        User newUser;
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            newUser = await CreateAsync(user, cancellationToken);

            userRole.UserId = newUser.UserId;
            _ = await _dbContext.UserOfficeRoles.AddAsync(userRole, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return newUser;
    }

    public async Task<User> CreateAsync(User value, CancellationToken cancellationToken = default)
    {
        User user;
        try
        {
            _ = await _dbContext.Users.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            user = value;
        }
        catch (Exception)
        {
            throw;
        }

        return user;
    }

    public Task<bool> DeleteAsync(long key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> Update(long key, User value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> Upsert(long key, User value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}