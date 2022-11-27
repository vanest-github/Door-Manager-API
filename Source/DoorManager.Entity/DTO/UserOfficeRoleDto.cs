using System;
using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class UserOfficeRoleDto
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public int OfficeId { get; set; }

    [Required]
    public int RoleId { get; set; }

    public long? OnBehalfUserId { get; set; }

    public DateTimeOffset? ValidFrom { get; set; }

    [Required]
    public DateTimeOffset ValidTo { get; set; }
}