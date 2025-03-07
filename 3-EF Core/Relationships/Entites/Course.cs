using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        // Navigation property for many-to-many relationship with Student  
        public List<StudentCourse> StudentCourses { get; set; }

        // Navigation property for one-to-many relationship with Professor  
        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }
    }
}
