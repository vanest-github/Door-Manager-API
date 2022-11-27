using System.ComponentModel.DataAnnotations;

namespace DoorManager.Entity.DTO;

public class UserDto
{
    [Required]
    public string RoleName { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public long LastModifiedBy { get; set; }
}