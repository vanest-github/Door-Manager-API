using System;
using System.ComponentModel.DataAnnotations;
using DoorManager.Entity.Enum;

namespace DoorManager.Entity.DTO;

public class DoorUnlockDto
{
    [Required]
    public int OfficeId { get; set; }

    [Required]
    public Guid DoorId { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public DoorAccessMode DoorAccessMode { get; set; }
}