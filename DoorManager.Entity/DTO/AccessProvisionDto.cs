using System;
using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class AccessProvisionDto
{
    [Required]
    public int OfficeId { get; set; }

    [Required]
    public string RoleName { get; set; }

    [Required]
    public string DoorType { get; set; }

    public DateTimeOffset? AccessFrom { get; set; }

    [Required]
    public DateTimeOffset AccessTo { get; set; }
}