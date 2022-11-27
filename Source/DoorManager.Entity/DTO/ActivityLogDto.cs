using System;
using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class ActivityLogDto
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public int OfficeId { get; set; }

    [Required]
    public DateTimeOffset ActivityTime { get; set; }

    [Required]
    public string Action { get; set; }

    public string Description { get; set; }
}