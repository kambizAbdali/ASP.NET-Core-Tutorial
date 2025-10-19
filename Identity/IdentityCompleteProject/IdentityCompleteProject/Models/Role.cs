using Microsoft.AspNetCore.Identity;

namespace IdentityCompleteProject.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<RolePermission> Permissions { get; set; }
    }
}
