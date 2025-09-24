using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyExample.Core.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Tags> Tags { get; set;}
    }
}
