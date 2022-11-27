using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("roles")]
public class Role
{
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; }

    [Column("role_priority")]
    public int RolePriority { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}