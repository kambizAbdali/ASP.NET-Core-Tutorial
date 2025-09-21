using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Core.Entities
{
    public class Product
    {
        public long Id { get; set; }
        [ConcurrencyCheck]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
