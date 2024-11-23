using System;
using System.Collections.Generic;
using System.Linq;
// Min-Max-Average-Sum-Aggregate
namespace LINQ_Aggregate
{
    public class Classroom
    {
        public string Name { get; set; }
        public int StudentCount { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // List of Classes
            var classrooms = new List<Classroom>
            {
                new Classroom { Name = "Math 101", StudentCount = 30 },
                new Classroom { Name = "Physics 101", StudentCount = 25 },
                new Classroom { Name = "Chemistry 101", StudentCount = 28 },
                new Classroom { Name = "Biology 101", StudentCount = 32 },
                new Classroom { Name = "History 101", StudentCount = 20 }
            };

            // Count of Classes
            int totalClassrooms = classrooms.Count();
            Console.WriteLine("--------------Count()-----------------");
            Console.WriteLine($"Total number of classrooms: {totalClassrooms}");

            // Average of Student Count
            double averageStudents = classrooms.Average(c => c.StudentCount);
            Console.WriteLine("--------------Average()-----------------");
            Console.WriteLine($"Average number of students per classroom: {averageStudents}");

            // Min of Student Count
            double minStudentCount = classrooms.Min(c => c.StudentCount);
            Console.WriteLine("--------------Min()-----------------");
            Console.WriteLine($"Min number of students per classroom: {minStudentCount}");

            // Max of Student Count
            double maxStudentCount = classrooms.Max(c => c.StudentCount);
            Console.WriteLine("--------------Max()-----------------");
            Console.WriteLine($"Max number of students per classroom: {maxStudentCount}");

            // Sum of Student Count
            double sumStudentCount = classrooms.Sum(c => c.StudentCount);
            Console.WriteLine("--------------Sum()-----------------");
            Console.WriteLine($"Sum of students per classroom: {sumStudentCount}");

            // Using Aggregate to calculate total number of students
            int totalStudents = classrooms.Aggregate(0, (total, cRoom) => total + cRoom.StudentCount);
            Console.WriteLine("--------------Aggregate()-----------------");
            Console.WriteLine($"Total number of students across all classrooms: {totalStudents}");

            // Class List with trailing comma
            string classNames = classrooms.Aggregate("Names: ", (classes, cRoom) => classes + cRoom.Name + ",");
            Console.WriteLine($"Class List: {classNames}");

            // Class List without trailing comma
            string classNames2 = classrooms
                .Select(cRoom => cRoom.Name)
                .Aggregate("Names: ", (classes, name) => classes + (classes == "Names: " ? "" : ", ") + name);
            Console.WriteLine($"Class List 2: {classNames2}");
        }
    }
}
