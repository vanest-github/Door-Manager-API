using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("doors")]
public class Door
{
    [Key]
    [Column("door_id")]
    public Guid DoorId { get; set; }

    [ForeignKey("office_id")]
    [Column("office_id")]
    public int OfficeId { get; set; }

    [ForeignKey("door_type_id")]
    [Column("door_type_id")]
    public int DoorTypeId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("current_status")]
    public string CurrentStatus { get; set; }

    [Column("manufacturer")]
    public string Manufacturer { get; set; }

    [Column("lock_version")]
    public string LockVersion { get; set; }

    [Column("created_time")]
    public DateTimeOffset CreatedTime { get; set; }

    [Column("modified_time")]
    public DateTimeOffset ModifiedTime { get; set; }
}