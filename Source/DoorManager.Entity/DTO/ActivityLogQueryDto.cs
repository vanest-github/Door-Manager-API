using System;

namespace DoorManager.Entity.DTO;

public class ActivityLogQueryDto
{
    public int? OfficeId { get; set; }

    public long? UserId { get; set; }

    public DateTimeOffset? FromTime { get; set; }

    public DateTimeOffset? ToTime { get; set; }
}