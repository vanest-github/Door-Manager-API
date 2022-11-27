using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("offices")]
public class Office
{
    [Key]
    [Column("office_id")]
    public int OfficeId { get; set; }

    [Column("office_name")]
    public string OfficeName { get; set; }

    [Column("latitude")]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    public decimal? Longitude { get; set; }

    [Column("country")]
    public string Country { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}