using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // Navigation property (for relationships) - Virtual for Lazy Loading

        public string FullName() => $"{Username} - {Email}";

        public string Username { get; set; }
        public string Email { get; set; }

        public Address Home { get; set; }
        public Address WorkPlace { get; set; }
    }
}
