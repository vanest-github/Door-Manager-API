using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("door_types")]
public class DoorType
{
    [Key]
    [Column("door_type_id")]
    public int DoorTypeId { get; set; }

    [Column("door_type")]
    public string DoorTypeName { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}