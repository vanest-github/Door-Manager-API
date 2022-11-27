using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("door_access_roles")]
public class DoorAccessRole
{
    [Key]
    [Column("door_access_id")]
    public long DoorAccessId { get; set; }

    [ForeignKey("door_type_id")]
    [Column("door_type_id")]
    public int DoorTypeId { get; set; }

    [ForeignKey("office_id")]
    [Column("office_id")]
    public int OfficeId { get; set; }

    [ForeignKey("role_id")]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("access_from")]
    public DateTimeOffset AccessFrom { get; set; }

    [Column("access_to")]
    public DateTimeOffset AccessTo { get; set; }
}