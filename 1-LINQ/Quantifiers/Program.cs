using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Quantifiers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize a list of products  
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", CategoryId = 1, Price = 150 },
                new Product { Id = 2, Name = "Product B", CategoryId = 1, Price = 200 },
                new Product { Id = 3, Name = "Product C", CategoryId = 2, Price = 90 },
                new Product { Id = 3, Name = "Product C", CategoryId = 2, Price = 90 }
            };


            // Check if all products have a price greater than 89  
            CheckAllProductsAbovePrice(products, 89);

            // Check if there are any products with price below 50  
            CheckAnyProductBelowPrice(products, 50);

            // Check if there is a product with Id 1  
            CheckProductExistsById(products, 1);

            // Wait for user input before closing  
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /*--------------------------All---------------------------*/
        static void CheckAllProductsAbovePrice(List<Product> products, decimal priceThreshold)
        {
            bool allAboveThreshold = products.All(p => p.Price > priceThreshold);
            Console.WriteLine($"All products above {priceThreshold}: {allAboveThreshold}"); // خروجی: False  
        }

        /*--------------------------Any---------------------------*/
        static void CheckAnyProductBelowPrice(List<Product> products, decimal priceThreshold)
        {
            bool anyBelowThreshold = products.Any(p => p.Price < priceThreshold);
            Console.WriteLine($"Any product below {priceThreshold}: {anyBelowThreshold}"); // خروجی: False  
        }

        /*--------------------------Select---------------------------*/
        static void CheckProductExistsById(List<Product> products, int id)
        {
            bool containsProduct = products.Select(p => p.Id).Contains(id);
            Console.WriteLine($"Product with Id {id} exists: {containsProduct}"); // خروجی: True  
        }
    }
}