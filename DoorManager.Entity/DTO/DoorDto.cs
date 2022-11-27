using System.ComponentModel.DataAnnotations;
using DoorManager.Entity.Enum;

namespace DoorManager.Entity.DTO;

public class DoorDto
{
    [Required]
    public string OfficeName { get; set; }

    [Required]
    public string DoorType { get; set; }

    [Required]
    public DoorStatus DoorStatus { get; set; }

    public string Manufacturer { get; set; }

    public string LockVersion { get; set; }
}