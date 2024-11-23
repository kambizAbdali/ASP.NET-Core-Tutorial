using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GroupedProductInfo
    {
        public string CategoryName { get; set; }
        public List<string> Products { get; set; }
        public int ProductCount { get; set; }
    }
}
