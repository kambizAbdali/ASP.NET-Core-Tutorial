using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingRelationships.Core.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDescription { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; } // Navigation property - Virtual for Lazy Loading
    }
}
