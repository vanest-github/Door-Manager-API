using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorManager.Entity.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum DoorStatus
{
    [EnumMember(Value = "Closed")]
    Closed = 0,

    [EnumMember(Value = "Open")]
    Open = 1,

    [EnumMember(Value = "InActive")]
    InActive = 2,

    [EnumMember(Value = "NotInUse")]
    NotInUse = 3
}