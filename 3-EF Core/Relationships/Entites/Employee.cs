using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Position { get; set; } // سمت شغلی  
        public int? ProfessorId { get; set; } // Foreign Key به Professor (nullable)  
        public Professor Professor { get; set; } // Navigation Property به Professor  
    }
}
