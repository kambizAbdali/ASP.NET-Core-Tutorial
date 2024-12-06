using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQ_Conversion
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    internal class Program
    {

        static void Main(string[] args)
        {
            // Sample list of students  
            List<Student> students = new List<Student>
        {
            new Student { Id = 1, Name = "Ali" },
            new Student { Id = 2, Name = "Sara" },
            new Student { Id = 3, Name = "Zahra" },
            new Student { Id = 4, Name = "Mohammad" }
        };

            // 1. Using AsEnumerable  
            // AsEnumerable converts a collection to IEnumerable.  
            // This method performs operations in memory and is suitable for working with data.  
            Console.WriteLine("Using AsEnumerable:");
            var enumerableStudents = students.AsEnumerable();
            foreach (var student in enumerableStudents)
            {
                Console.WriteLine(student.Name);
            }
            // Output:  
            // Using AsEnumerable:  
            // Ali  
            // Sara  
            // Zahra  
            // Mohammad  

            // 2. Using AsQueryable  
            // AsQueryable converts a collection to IQueryable.  
            // This method allows using LINQ to perform filtering on data.  
            Console.WriteLine("\nUsing AsQueryable:");
            IQueryable<Student> queryableStudents = students.AsQueryable();
            var filteredStudents = queryableStudents.Where(s => s.Id > 2);
            foreach (var student in filteredStudents)
            {
                Console.WriteLine(student.Name);
            }
            // Output:  
            // Using AsQueryable:  
            // Zahra  
            // Mohammad  

            // 3. Using Cast  
            // Cast converts objects in a collection to a specific type.  
            // If the type is invalid, an exception is thrown.  
            Console.WriteLine("\nUsing Cast:");
            IEnumerable<object> objects = students.Cast<object>();
            foreach (var obj in objects)
            {
                Console.WriteLine(((Student)obj).Name);
            }
            // Output:  
            // Using Cast:  
            // Ali  
            // Sara  
            // Zahra  
            // Mohammad  

            // 4. Using OfType  
            // OfType selects only objects of a specific type and ignores invalid objects.  
            Console.WriteLine("\nUsing OfType:");
            IEnumerable<object> mixedObjects = new List<object> { new Student { Id = 1, Name = "Ali" }, "Not a Student" , 19};
            var validStudents = mixedObjects.OfType<Student>();
            foreach (var student in validStudents)
            {
                Console.WriteLine(student.Name);
            }
            // Output:  
            // Using OfType:  
            // Ali  

            // 5. Using ToArray  
            // ToArray converts a collection to an array.  
            Console.WriteLine("\nUsing ToArray:");
            Student[] studentArray = students.ToArray();
            foreach (var student in studentArray)
            {
                Console.WriteLine(student.Name);
            }
            // Output:  
            // Using ToArray:  
            // Ali  
            // Sara  
            // Zahra  
            // Mohammad  

            // 6. Using ToDictionary  
            // ToDictionary converts a collection to a dictionary.  
            // A unique key must be specified for each element.  
            Console.WriteLine("\nUsing ToDictionary:");
            Dictionary<int, Student> studentDictionary = students.ToDictionary(s => s.Id);
            foreach (var kvp in studentDictionary)
            {
                Console.WriteLine($"Id: {kvp.Key}, Name: {kvp.Value.Name}");
            }
            // Output:  
            // Using ToDictionary:  
            // Id: 1, Name: Ali  
            // Id: 2, Name: Sara  
            // Id: 3, Name: Zahra  
            // Id: 4, Name: Mohammad  

            // 7. Using ToList  
            // ToList converts a collection to a mutable list.  
            Console.WriteLine("\nUsing ToList:");
            List<Student> studentList = students.ToList();
            foreach (var student in studentList)
            {
                Console.WriteLine(student.Name);
            }
            // Output:  
            // Using ToList:  
            // Ali  
            // Sara  
            // Zahra  
            // Mohammad  

            // 8. Using ToLookup  
            // ToLookup works similarly to a dictionary but can have duplicate keys.  
            // This method allows grouping objects by a specific key.  
            Console.WriteLine("\nUsing ToLookup:");
            var studentLookup = students.ToLookup(s => s.Name.First());
            foreach (var group in studentLookup)
            {
                Console.WriteLine($"Students with initial '{group.Key}':");
                foreach (var student in group)
                {
                    Console.WriteLine(student.Name);
                }
            }
            // Output:  
            // Using ToLookup:  
            // Students with initial 'A':  
            // Ali  
            // Students with initial 'S':  
            // Sara  
            // Students with initial 'Z':  
            // Zahra  
            // Students with initial 'M':  
            // Mohammad  
            Console.ReadKey();
        }
    }
}