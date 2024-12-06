using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Generation
{
    public class Student
    {
        public string Name { get; set; }

        public Student(string name)
        {
            Name = name;
        }
    }

    class Program
    {
        static void Main()
        {
            // Demonstrating DefaultIfEmpty with List<string>  
            List<string> strings = new List<string>();
            var defaultStringResult = strings.DefaultIfEmpty("No values");
            Console.WriteLine("DefaultIfEmpty with List<string>: " + string.Join(", ", defaultStringResult));

            // Demonstrating DefaultIfEmpty with List<Student>  
            List<Student> students = new List<Student>();
            var defaultStudentResult = students.DefaultIfEmpty(new Student("No students"));
            Console.WriteLine("DefaultIfEmpty with List<Student>: " + string.Join(", ", defaultStudentResult.Select(s => s.Name)));

            // Demonstrating Empty with IEnumerable<string>  
            var emptyStrings = Enumerable.Empty<string>();
            Console.WriteLine($"Count of emptyStrings: {emptyStrings.Count()}");

            // Demonstrating Empty with IEnumerable<Student>  
            var emptyStudents = Enumerable.Empty<Student>();
            Console.WriteLine($"Count of emptyStudents: {emptyStudents.Count()}");

            // Demonstrating Range with List<string>  
            var numbers = Enumerable.Range(1, 5).Select(n => n.ToString()).ToList();
            Console.WriteLine("Range with List<string>: " + string.Join(", ", numbers));

            // Demonstrating Range with List<Student>  
            var studentList = Enumerable.Range(1, 5)
                .Select(n => new Student($"Student {n}"))
                .ToList();
            Console.WriteLine("Range with List<Student>: " + string.Join(", ", studentList.Select(s => s.Name)));

            // Demonstrating Repeat with List<string>  
            var repeatedStrings = Enumerable.Repeat("Repeated value", 3).ToList();
            Console.WriteLine("Repeat with List<string>: " + string.Join(", ", repeatedStrings));

            // Demonstrating Repeat with List<Student>  
            var repeatedStudents = Enumerable.Repeat(new Student("Repeated Student"), 3).ToList();
            Console.WriteLine("Repeat with List<Student>: " + string.Join(", ", repeatedStudents.Select(s => s.Name)));

            Console.ReadKey(); // Keep the console open  
        }
    }
}