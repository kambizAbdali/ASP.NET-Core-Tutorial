using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Exam
    {
        public int ExamId { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int CourseId { get; set; } // Foreign Key به Course  
        public Course Course { get; set; } // Navigation Property به Course  
        public List<StudentExam> StudentExams { get; set; }
    }
}
