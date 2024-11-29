using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Set_Cancat
{
    public class User
    {
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(User user) => user.Name?.GetHashCode() ?? 0;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DisplayDistinctNumbers();
            DisplayUserOperations();

            Console.ReadKey();
        }

        private static void DisplayDistinctNumbers()
        {
            List<int> numbers = new List<int> { 1, 2, 2, 3, 4, 4, 5, 5, 7 };
            Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");
            Console.WriteLine("\nDistinct Numbers:");
            foreach (var number in numbers.Distinct())
            {
                Console.WriteLine(number);
            }

            int[] groupOne = { 1, 2, 3, 4, 5 };
            int[] groupTwo = { 4, 5, 6, 7, 8 };
            Console.WriteLine($"\nGroup One: {string.Join(", ", groupOne)}");
            Console.WriteLine($"Group Two: {string.Join(", ", groupTwo)}");

            DisplayDifferences(groupOne, groupTwo);
            DisplayIntersections(groupOne, groupTwo);
            DisplayUnions(groupOne, groupTwo);
            DisplayConcatenation(groupOne, groupTwo);
        }

        private static void DisplayDifferences(int[] groupOne, int[] groupTwo)
        {
            Console.WriteLine("\nDifference (Group One - Group Two):");
            foreach (var number in groupOne.Except(groupTwo))
            {
                Console.WriteLine(number);
            }
        }

        private static void DisplayIntersections(int[] groupOne, int[] groupTwo)
        {
            Console.WriteLine("\nIntersection (Group One ∩ Group Two):");
            foreach (var number in groupOne.Intersect(groupTwo))
            {
                Console.WriteLine(number);
            }
        }

        private static void DisplayUnions(int[] groupOne, int[] groupTwo)
        {
            Console.WriteLine("\nUnion (Group One ∪ Group Two):");
            foreach (var number in groupOne.Union(groupTwo))
            {
                Console.WriteLine(number);
            }
        }

        private static void DisplayConcatenation(int[] groupOne, int[] groupTwo)
        {
            Console.WriteLine("\nConcatenation (Group One + Group Two):");
            foreach (var number in groupOne.Concat(groupTwo))
            {
                Console.WriteLine(number);
            }
        }

        private static void DisplayUserOperations()
        {
            List<User> users = new List<User>
            {
                new User { Name = "Alice" },
                new User { Name = "Bob" },
                new User { Name = "Alice" },
                new User { Name = "David" },
                new User { Name = "Eve" },
                new User { Name = "Bob" }
            };

            Console.WriteLine($"Users: {string.Join(", ", users.Select(u => u.Name))}");
            Console.WriteLine("\nDistinct Users:");
            foreach (var user in users.Distinct(new UserComparer()))
            {
                Console.WriteLine(user);
            }

            User[] groupOne = { new User { Name = "Alice" }, new User { Name = "Bob" }, new User { Name = "Charlie" } };
            User[] groupTwo = { new User { Name = "Bob" }, new User { Name = "David" }, new User { Name = "Eve" }, new User { Name = "Frank" } };

            Console.WriteLine($"\nGroup One: {string.Join(", ", groupOne.Select(u => u.Name))}");
            Console.WriteLine($"Group Two: {string.Join(", ", groupTwo.Select(u => u.Name))}");

            DisplayUserDifferences(groupOne, groupTwo);
            DisplayUserIntersections(groupOne, groupTwo);
            DisplayUserUnions(groupOne, groupTwo);
            DisplayUserConcatenation(groupOne, groupTwo);
        }

        private static void DisplayUserDifferences(User[] groupOne, User[] groupTwo)
        {
            Console.WriteLine("\nUser Difference (Group One - Group Two):");
            foreach (var user in groupOne.Except(groupTwo, new UserComparer()))
            {
                Console.WriteLine(user);
            }
        }

        private static void DisplayUserIntersections(User[] groupOne, User[] groupTwo)
        {
            Console.WriteLine("\nUser Intersection (Group One ∩ Group Two):");
            foreach (var user in groupOne.Intersect(groupTwo, new UserComparer()))
            {
                Console.WriteLine(user);
            }
        }

        private static void DisplayUserUnions(User[] groupOne, User[] groupTwo)
        {
            Console.WriteLine("\nUser Union (Group One ∪ Group Two):");
            foreach (var user in groupOne.Union(groupTwo, new UserComparer()))
            {
                Console.WriteLine(user);
            }
        }

        private static void DisplayUserConcatenation(User[] groupOne, User[] groupTwo)
        {
            Console.WriteLine("\nUser Concatenation (Group One + Group Two):");
            foreach (var user in groupOne.Concat(groupTwo))
            {
                Console.WriteLine(user);
            }
        }
    }
}