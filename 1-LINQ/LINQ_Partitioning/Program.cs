using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Partitioning
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1, 3, 4, 8, 2, 7, 5 };

            // Demonstrating Skip  
            Console.WriteLine("------------- Skip -------------");
            var skipResult = numbers.Skip(2); // Ignores the first two elements  
            Console.WriteLine(string.Join(", ", skipResult));

            // Demonstrating SkipWhile  
            Console.WriteLine("----------- SkipWhile -----------");
            var skipWhileResult = numbers.SkipWhile(n => n < 3); // Ignores elements while they are less than 3  
            Console.WriteLine(string.Join(", ", skipWhileResult));

            // Demonstrating Take  
            Console.WriteLine("------------- Take -------------");
            var takeResult = numbers.Take(3); // Takes the first three elements  
            Console.WriteLine(string.Join(", ", takeResult));

            // Demonstrating TakeWhile  
            Console.WriteLine("---------- TakeWhile ----------");
            var takeWhileResult = numbers.TakeWhile(n => n < 4); // Takes elements while they are less than 4  
            Console.WriteLine(string.Join(", ", takeWhileResult));


            // Demonstrating SequenceEqual  
            Console.WriteLine("---------- SequenceEqual ----------");
            List<int> list1 = new List<int> { 1, 2, 3, 4, 5 };
            List<int> list2 = new List<int> { 1, 2, 3, 4, 5 };

            bool areEqual = list1.SequenceEqual(list2);
            Console.WriteLine($"Are list1 and list2 equal? {areEqual}"); // خروجی: True  



            List<int> list3 = new List<int> { 1, 2, 3 };
            List<int> list4 = new List<int> { 1, 2, 4 };

            areEqual = list3.SequenceEqual(list4);
            Console.WriteLine($"Are list3 and list4 equal? {areEqual}"); // خروجی: False
            Console.ReadKey();
        }
    }
}