using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }

        // Navigation property for many-to-many relationship with Course  
        public List<StudentCourse> StudentCourses { get; set; }

        // Navigation property for one-to-many relationship with Advisor (استاد راهنما)  
        public int? AdvisorId { get; set; }
        public Professor Advisor { get; set; }
        public List<StudentExam> StudentExams { get; set; }
    }
}
