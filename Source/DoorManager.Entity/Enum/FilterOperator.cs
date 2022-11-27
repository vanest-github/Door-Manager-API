using System.Runtime.Serialization;

namespace DoorManager.Entity.Enum;

public enum FilterOperator
{
    [EnumMember(Value = "Equal")]
    Equal,

    [EnumMember(Value = "NotEqual")]
    NotEqual,

    [EnumMember(Value = "In")]
    In,

    [EnumMember(Value = "Like")]
    Like,

    [EnumMember(Value = "Between")]
    Between,
}
