using Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Employee> Employees { get; set; } // اضافه کردن Employee  
        public DbSet<Exam> Exams { get; set; } // اضافه کردن Exam  
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; } // اضافه کردن StudentExam  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure many-to-many relationship between Student and Course  
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });


            base.OnModelCreating(modelBuilder);

            // تنظیم کلید ترکیبی برای StudentExam  
            modelBuilder.Entity<StudentExam>()
                .HasKey(se => new { se.StudentId, se.ExamId });

            // رابطه بین Professor و Employee  
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.Employee)
                .WithOne(e => e.Professor)
                .HasForeignKey<Employee>(e => e.ProfessorId)
                .OnDelete(DeleteBehavior.SetNull);

            // رابطه بین StudentExam و Student  
            modelBuilder.Entity<StudentExam>()
                .HasOne(se => se.Student)
                .WithMany(s => s.StudentExams)
                .HasForeignKey(se => se.StudentId);

            // رابطه بین StudentExam و Exam  
            modelBuilder.Entity<StudentExam>()
                .HasOne(se => se.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(se => se.ExamId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // Configure one-to-many relationship between Professor and Course  
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between Professor and Student (as Advisor)  
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.AdvisorId)
                .OnDelete(DeleteBehavior.NoAction); // Or DeleteBehavior.Cascade  

            // تنظیم Shadow Properties برای Course  
            modelBuilder.Entity<Course>()
                .Property<DateTime>("CreatedDate");
            modelBuilder.Entity<Course>()
                .Property<DateTime>("UpdatedDate");

            // تنظیم Shadow Properties برای Employee  
            modelBuilder.Entity<Employee>()
                .Property<DateTime>("CreatedDate");
            modelBuilder.Entity<Employee>()
                .Property<DateTime>("UpdatedDate");

            // تنظیم Shadow Properties برای Exam  
            modelBuilder.Entity<Exam>()
                .Property<DateTime>("CreatedDate");
            modelBuilder.Entity<Exam>()
                .Property<DateTime>("UpdatedDate");

            // تنظیم Shadow Properties برای Professor  
            modelBuilder.Entity<Professor>()
                .Property<DateTime>("CreatedDate");
            modelBuilder.Entity<Professor>()
                .Property<DateTime>("UpdatedDate");

        }
    }
}