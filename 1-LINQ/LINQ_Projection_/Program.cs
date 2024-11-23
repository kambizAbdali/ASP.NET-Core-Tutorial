using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQ_Projection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Sample product data
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", CategoryId = 1 },
                new Product { Id = 2, Name = "Smartphone", CategoryId = 1 },
                new Product { Id = 3, Name = "Refrigerator", CategoryId = 2 },
                new Product { Id = 4, Name = "Washing Machine", CategoryId = 2 },
                new Product { Id = 5, Name = "Sofa", CategoryId = 3 },
                new Product { Id = 6, Name = "Dining Table", CategoryId = 3 },
                new Product { Id = 7, Name = "T-shirt", CategoryId = 4 },
                new Product { Id = 8, Name = "Jeans", CategoryId = 4 }
            };

            // Sample category data
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Home Appliances" },
                new Category { Id = 3, Name = "Furniture" },
                new Category { Id = 4, Name = "Clothing" }
            };

            // Join Products and Categories using Method Syntax with Projection
            var joinMethodSyntax = products.Join(categories,
                product => product.CategoryId,
                category => category.Id,
                (product, category) => new
                {
                    ProductName = product.Name,
                    CategoryName = category.Name,
                    Description = $"{product.Name} is a product in the {category.Name} category."
                });

            Console.WriteLine("------------ Join Result (Method Syntax with Projection) -----------");
            foreach (var item in joinMethodSyntax)
            {
                  Console.WriteLine($"Product: {item.ProductName}, Category: {item.CategoryName}, Description: {item.Description}");
                 }
                 
                 
                 // We can map a specific model to Projection, For example GroupedProductInfo
                 // Group Products by Category using Method Syntax with Projection to a specific model
                 var groupedMethodSyntax = categories
                     .Join(products,
                         category => category.Id,
                         product => product.CategoryId,
                         (category, product) => new
                         {
                             CategoryName = category.Name,
                             ProductName = product.Name
                         })
                     .GroupBy(x => x.CategoryName)
                     .Select(g => new GroupedProductInfo
                     {
                         CategoryName = g.Key,
                         Products = g.Select(p => p.ProductName).ToList(),
                         ProductCount = g.Count()
                     });

                 Console.WriteLine("\n------------ Grouped Result (Method Syntax with Projection) -----------");
                 foreach (var group in groupedMethodSyntax)
                 {
                     Console.WriteLine($"Category: {group.CategoryName}, Product Count: {group.ProductCount}");
                     Console.WriteLine($"Products: { string.Join(", ", group.Products)}");
                 }
                 
            /*-----------------------SelectMany------------------------*/
            // Use SelectMany to associate each product with all categories
            var categoryProducts = categories.SelectMany(category =>
                products.Where(product => product.CategoryId == category.Id),
                (category, product) => new
                {
                    CategoryName = category.Name,
                    ProductName = product.Name
                });
            
            Console.WriteLine("\n\n------------ Category Products: SelectMany -----------");
            foreach (var pair in categoryProducts)
            {
                Console.WriteLine($"Product: {pair.ProductName}, Category: {pair.CategoryName}");
            }
        }   
    }       
}           