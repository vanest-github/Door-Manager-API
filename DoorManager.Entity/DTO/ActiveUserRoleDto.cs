using System;

namespace DoorManager.Entity.DTO;

public class ActiveUserRoleDto
{
    public int OfficeId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public int RolePriority { get; set; }

    public long? OnBehalfUserId { get; set; }

    public DateTimeOffset ValidFrom { get; set; }

    public DateTimeOffset ValidTo { get; set; }
}