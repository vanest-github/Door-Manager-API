using DoorManager.Entity;
using Microsoft.EntityFrameworkCore;

namespace DoorManager.Storage.MySql;

public class DoorManagerDbContext : DbContext
{
    public DoorManagerDbContext(DbContextOptions<DoorManagerDbContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<ActivityLog> ActivityLogs { get; set; }

    public DbSet<Door> Doors { get; set; }

    public DbSet<DoorAccessRole> DoorAccessRoles { get; set; }

    public DbSet<DoorType> DoorTypes { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<RoleFeature> RoleFeatures { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserOfficeRole> UserOfficeRoles { get; set; }

    public DbSet<Office> Offices { get; set; }
}
