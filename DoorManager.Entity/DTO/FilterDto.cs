using System.ComponentModel.DataAnnotations;
using DoorManager.Entity.Enum;

namespace DoorManager.Entity.DTO;

public class FilterDto
{
    [Required]
    public string ColumnName { get; set; }

    public FilterOperator Operator { get; set; }

    public string Values { get; set; }
}
