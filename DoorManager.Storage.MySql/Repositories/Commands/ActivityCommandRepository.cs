using DoorManager.Entity;
using DoorManager.Storage.Interface.Commands;

namespace DoorManager.Storage.MySql.Repositories.Commands;

public class ActivityCommandRepository : IActivityCommandRepository
{
    private readonly DoorManagerDbContext _dbContext;

    public ActivityCommandRepository(DoorManagerDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<ActivityLog> CreateAsync(ActivityLog value, CancellationToken cancellationToken = default)
    {
        ActivityLog activityLog;
        try
        {
            _ = await _dbContext.ActivityLogs.AddAsync(value, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            activityLog = value;
        }
        catch (Exception)
        {
            throw;
        }

        return activityLog;
    }
}