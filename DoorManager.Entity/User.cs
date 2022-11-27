using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("last_access_time")]
    public DateTimeOffset? LastAccessTime { get; set; }

    [Column("last_access_type")]
    public string LastAccessType { get; set; }
}