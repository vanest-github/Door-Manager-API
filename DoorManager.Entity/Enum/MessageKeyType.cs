using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoorManager.Entity.Enum;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageKeyType
{
    [EnumMember(Value = "ErrorKey")]
    ErrorKey = 0,

    [EnumMember(Value = "SuccessKey")]
    SuccessKey = 1
}