using System;
using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class AccessDelegationDto
{
    [Required]
    public int OfficeId { get; set; }

    [Required]
    public long IssuingUserId { get; set; }

    [Required]
    public long TargetUserId { get; set; }

    [Required]
    public int RoleId { get; set; }

    public DateTimeOffset? ValidFrom { get; set; }

    [Required]
    public DateTimeOffset ValidTo { get; set; }
}