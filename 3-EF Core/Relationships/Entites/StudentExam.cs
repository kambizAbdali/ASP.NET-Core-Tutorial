using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class StudentExam
    {
        public int StudentId { get; set; } // Foreign Key به Student  
        public Student Student { get; set; } // Navigation Property به Student  
        public int ExamId { get; set; } // Foreign Key به Exam  
        public Exam Exam { get; set; } // Navigation Property به Exam  
        public int Score { get; set; } // نمره دانش‌آموز در آزمون  

        // کلید ترکیبی (Composite Key)  
        public StudentExam()
        {
        }
    }
}