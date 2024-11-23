using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ__Elements__
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", CategoryId = 1, Price = 150 },
                new Product { Id = 2, Name = "Product B", CategoryId = 1, Price = 200 },
                new Product { Id = 3, Name = "Product C", CategoryId = 2, Price = 90 },  
                // Adding a product with the same Id for testing Single and SingleOrDefault  
                new Product { Id = 3, Name = "Product D", CategoryId = 2, Price = 100 }
            };

            Console.WriteLine("Display a product at a specific index:");
            DisplayProductAtIndex(products, 1); // Shows the product at index 1  
            DisplayProductAtIndex(products, 0); // Shows the product at index 0  
            DisplayProductAtIndex(products, 3); // Attempt to show product at an invalid index  

            Console.WriteLine("\nUsing ElementAtOrDefault:");
            DisplayProductAtIndexOrDefault(products, 1); // Shows the product at index 1  
            DisplayProductAtIndexOrDefault(products, 0); // Shows the product at index 0  
            DisplayProductAtIndexOrDefault(products, 3); // Attempt to show product at an invalid index   

            Console.WriteLine("\nUsing First and FirstOrDefault:");
            DisplayFirstProduct(products);
            DisplayFirstProductOrDefault(products, 5); // Attempting to retrieve a product from an empty list  

            Console.WriteLine("\nUsing Last and LastOrDefault:");
            DisplayLastProduct(products);
            DisplayLastProductOrDefault(new List<Product>()); // Attempt for an empty list  

            Console.WriteLine("\nUsing Single and SingleOrDefault:");
            DisplaySingleProduct(products);
            DisplaySingleProductOrDefault(products); // Expected to fail due to multiple products with Id 3  

            // Wait for user input before closing  
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        // Displaying a product at a specified index using ElementAt  
        static void DisplayProductAtIndex(List<Product> products, int index)
        {
            try
            {
                Product product = products.ElementAt(index);
                Console.WriteLine($"Product at index {index}: {product.Name}, Price: {product.Price}");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine($"No product found at index {index}. Please provide a valid index (0 to {products.Count - 1}).");
            }
        }

        // Displaying a product at a specified index using ElementAtOrDefault  
        static void DisplayProductAtIndexOrDefault(List<Product> products, int index)
        {
            Product product = products.ElementAtOrDefault(index);
            if (product != null)
            {
                Console.WriteLine($"Product at index {index}: {product.Name}, Price: {product.Price}");
            }
            else
            {
                Console.WriteLine($"No product found at index {index}. Returned default value (null).");
            }
        }

        // Display the first product using First  
        static void DisplayFirstProduct(List<Product> products)
        {
            try
            {
                Product product = products.First();
                Console.WriteLine($"First product: {product.Name}, Price: {product.Price}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No products available.");
            }
        }

        // Display the first product using FirstOrDefault  
        static void DisplayFirstProductOrDefault(List<Product> products, int index)
        {
            Product product = products.FirstOrDefault();
            if (product != null)
            {
                Console.WriteLine($"First product: {product.Name}, Price: {product.Price}");
            }
            else
            {
                Console.WriteLine("No products available. Returned default value (null).");
            }
        }

        // Display the last product using Last  
        static void DisplayLastProduct(List<Product> products)
        {
            try
            {
                Product product = products.Last();
                Console.WriteLine($"Last product: {product.Name}, Price: {product.Price}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No products available.");
            }
        }

        // Display the last product using LastOrDefault  
        static void DisplayLastProductOrDefault(List<Product> products)
        {
            Product product = products.LastOrDefault();
            if (product != null)
            {
                Console.WriteLine($"Last product: {product.Name}, Price: {product.Price}");
            }
            else
            {
                Console.WriteLine("No products available. Returned default value (null).");
            }
        }

        // Display a single product using Single  
        static void DisplaySingleProduct(List<Product> products)
        {
            try
            {
                // Example: trying to retrieve a product with a specific condition, here we use Id 3 as an example  
                Product product = products.Single(p => p.Id == 3);
                Console.WriteLine($"Single product with Id 3: {product.Name}, Price: {product.Price}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("There is either no product or more than one product with the specified criteria.");
            }
        }

        // Display a single product using SingleOrDefault  
        static void DisplaySingleProductOrDefault(List<Product> products)
        {
            Product product = products.SingleOrDefault(p => p.Id == 999); // Id not in the list  
            if (product != null)
            {
                Console.WriteLine($"Single product with Id 999: {product.Name}, Price: {product.Price}");
            }
            else
            {
                Console.WriteLine("No product found with the specified criteria. Returned default value (null).");
            }
        }
    }
}