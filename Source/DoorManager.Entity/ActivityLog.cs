using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("activity_logs")]
public class ActivityLog
{
    [Key]
    [Column("activity_id")]
    public long ActivityId { get; set; }

    [ForeignKey("user_id")]
    [Column("user_id")]
    public long UserId { get; set; }

    [ForeignKey("office_id")]
    [Column("office_id")]
    public int OfficeId { get; set; }

    [Column("activity_time")]
    public DateTimeOffset ActivityTime { get; set; }

    [Column("action")]
    public string Action { get; set; }

    [Column("description")]
    public string Description { get; set; }
}