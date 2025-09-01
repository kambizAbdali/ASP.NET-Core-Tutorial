using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueConverterDemo.Core.Models
{
    public class Note
    {
        [Key]  // Indicates this property is the primary key.
        public int Id { get; set; }
        public string SecretNote { get; set; }
    }
}
