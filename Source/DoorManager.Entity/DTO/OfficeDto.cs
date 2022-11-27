using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class OfficeDto
{
    [Required]
    public string OfficeName { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [Required]
    public string Country { get; set; }

    public bool IsActive { get; set; }
}