using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Professor
    {
        public int ProfessorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Department { get; set; }
        public List<Course> Courses { get; set; }
        public List<Student> Students { get; set; }
        public int? EmployeeId { get; set; } // Foreign Key به Employee (nullable)  
        public Employee Employee { get; set; } // Navigation Property به Employee  
    }
}
