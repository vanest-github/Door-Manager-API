using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorManager.Entity.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum ActivityLogType
{
    [EnumMember(Value = "SelfAccess")]
    SelfAccess = 1,

    [EnumMember(Value = "DelegatedAccess")]
    DelegatedAccess = 2,

    [EnumMember(Value = "AccessProvision")]
    AccessProvision = 3,

    [EnumMember(Value = "AccessDelegation")]
    AccessDelegation = 4,

    [EnumMember(Value = "CreateOffice")]
    CreateOffice = 5,

    [EnumMember(Value = "CreateDoor")]
    CreateDoor = 6,

    [EnumMember(Value = "CreateUser")]
    CreateUser = 7
}