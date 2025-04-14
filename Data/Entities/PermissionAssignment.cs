
namespace Data.Entities
{
    public class PermissionAssignment : BaseEntities
    {
        // Foreign Keys
        public Guid RoleId { get; set; }
        public Role Roles { get; set; } = null!;
        public Guid PermissionId { get; set; }
        public Permission Permissions { get; set; } = null!;
    }
}
