using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorManager.Entity.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum DoorAccessMode
{
    [EnumMember(Value = "SelfAccess")]
    SelfAccess = 1,

    [EnumMember(Value = "DelegatedAccess")]
    DelegatedAccess = 2
}