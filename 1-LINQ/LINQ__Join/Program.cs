using LINQ_Join.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Join
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

            // Join Products and Categories using Method Syntax
            var joinMethodSyntax = products.Join(categories,
                product => product.CategoryId,
                category => category.Id,
                (product, category) => new
                {
                    ProductName = product.Name,
                    CategoryName = category.Name
                });

            Console.WriteLine("------------ Join Result (Method Syntax) -----------");
            foreach (var item in joinMethodSyntax)
            {
                Console.WriteLine($"Product: {item.ProductName}, Category: {item.CategoryName}");
            }

            // Join Products and Categories using Query Syntax
            var joinQuerySyntax = from product in products
                                  join category in categories on product.CategoryId equals category.Id
                                  select new
                                  {
                                      ProductName = product.Name,
                                      CategoryName = category.Name
                                  };

            Console.WriteLine("\n------------ Join Result (Query Syntax) -----------");
            foreach (var item in joinQuerySyntax)
            {
                Console.WriteLine($"Product: {item.ProductName}, Category: {item.CategoryName}");
            }

            // Group Products by Category using Method Syntax
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
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Products = g.Select(p => p.ProductName).ToList()
                });

            Console.WriteLine("\n------------ Group by Result (Method Syntax) -----------");
            foreach (var group in groupedMethodSyntax)
            {
                Console.WriteLine($"Category: {group.CategoryName}");
                Console.WriteLine("Products: " + string.Join(", ", group.Products));
            }

            // Group Products by Category using Query Syntax
            var groupedQuerySyntax = from category in categories
                                     join product in products on category.Id equals product.CategoryId
                                     group product by category.Name into productGroup
                                     select new
                                     {
                                         CategoryName = productGroup.Key,
                                         Products = productGroup.Select(p => p.Name).ToList()
                                     };

            Console.WriteLine("\n------------ Group by Result (Query Syntax) -----------");
            foreach (var group in groupedQuerySyntax)
            {
                Console.WriteLine($"Category: {group.CategoryName}");
                Console.WriteLine("Products: " + string.Join(", ", group.Products));
            }
        }
    }
}
