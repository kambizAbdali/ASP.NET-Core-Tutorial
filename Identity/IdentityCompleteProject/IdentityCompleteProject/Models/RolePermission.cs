namespace IdentityCompleteProject.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string Permission { get; set; }
        public string Description { get; set; }

        // Navigation property
        public virtual Role Role { get; set; }
    }
}
