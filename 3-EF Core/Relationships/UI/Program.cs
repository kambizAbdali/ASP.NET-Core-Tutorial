// UILayer  
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder; // Added for WebApplication  
using Microsoft.AspNetCore.Hosting; // Added for WebApplication  
using Entites;
using DAL;

namespace UILayer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Part 1: EF Core Console App - (As before)  
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Server =.; Database = UniversityDb; Integrated Security = true");

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                // Ensure the database is created  
                context.Database.EnsureCreated();

                // Seed some data  
                if (!context.Students.Any())
                {
                    var professor1 = new Professor { Name = "Dr. Smith", Department = "Computer Science" };
                    var professor2 = new Professor { Name = "Dr. Johnson", Department = "Mathematics" };

                    var course1 = new Course { Title = "Introduction to Programming", Description = "A beginner-friendly course", Professor = professor1 };
                    var course2 = new Course { Title = "Calculus I", Description = "Fundamentals of calculus", Professor = professor2 };

                    var student1 = new Student { Name = "Alice", Email = "alice@example.com", Advisor = professor1 };
                    var student2 = new Student { Name = "Bob", Email = "bob@example.com", Advisor = professor2 };

                    context.Professors.AddRange(professor1, professor2);
                    context.Courses.AddRange(course1, course2);
                    context.Students.AddRange(student1, student2);

                    context.SaveChanges();

                    context.StudentCourses.Add(new StudentCourse { Student = student1, Course = course1 });
                    context.StudentCourses.Add(new StudentCourse { Student = student2, Course = course2 });
                    context.StudentCourses.Add(new StudentCourse { Student = student1, Course = course2 }); // Alice takes Calculus as well  
                    context.SaveChanges();
                }

                // Example of querying data  
                var students = context.Students
                    .Include(s => s.StudentCourses)
                        .ThenInclude(sc => sc.Course)
                            .ThenInclude(c => c.Professor)
                    .Include(s => s.Advisor)
                    .ToList();

                foreach (var student in students)
                {
                    Console.WriteLine($"Student: {student.Name}, Email: {student.Email}");
                    Console.WriteLine($"Advisor: {student.Advisor?.Name}, Department: {student.Advisor?.Department}");
                    Console.WriteLine("Courses:");
                    foreach (var studentCourse in student.StudentCourses)
                    {
                        Console.WriteLine($"- {studentCourse.Course.Title} (Professor: {studentCourse.Course.Professor?.Name})");
                    }
                    Console.WriteLine();
                }

                // Example of querying courses and their students  
                var courses = context.Courses
                    .Include(c => c.StudentCourses)
                        .ThenInclude(sc => sc.Student)
                    .Include(c => c.Professor)
                    .ToList();

                Console.WriteLine("Courses and their students:");
                foreach (var course in courses)
                {
                    Console.WriteLine($"Course: {course.Title}, Professor: {course.Professor?.Name}");
                    Console.WriteLine("Students:");
                    foreach (var studentCourse in course.StudentCourses)
                    {
                        Console.WriteLine($"- {studentCourse.Student.Name}");
                    }
                    Console.WriteLine();
                }
            }

            // Part 2: Minimal API Setup  
            var builder = WebApplication.CreateBuilder(args); //Correct Place  
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}