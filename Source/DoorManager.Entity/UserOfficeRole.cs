using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("user_office_roles")]
public class UserOfficeRole
{
    [Key]
    [Column("user_office_role_id")]
    public long UserOfficeRoleId { get; set; }

    [ForeignKey("user_id")]
    [Column("user_id")]
    public long UserId { get; set; }

    [ForeignKey("office_id")]
    [Column("office_id")]
    public int OfficeId { get; set; }

    [ForeignKey("role_id")]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("valid_from")]
    public DateTimeOffset ValidFrom { get; set; }

    [Column("valid_to")]
    public DateTimeOffset ValidTo { get; set; }

    [ForeignKey("user_id")]
    [Column("acting_for")]
    public long? OnBehalfUserId { get; set; }
}