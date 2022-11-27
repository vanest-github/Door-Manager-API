using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoorManager.Entity;

[Table("rbac_role_features")]
public class RoleFeature
{
    [Key]
    [Column("rbac_role_feature_id")]
    public int RoleFeatureId { get; set; }

    [ForeignKey("role_id")]
    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("feature_id")]
    public int FeatureId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}