using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorManager.Entity.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserRoleType
{
    [EnumMember(Value = "Self")]
    Self = 0,

    [EnumMember(Value = "ProxyUser")]
    ProxyUser = 1
}