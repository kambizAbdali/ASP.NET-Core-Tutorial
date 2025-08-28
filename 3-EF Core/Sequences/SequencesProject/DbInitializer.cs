using Sequences.Data;
using Sequences.Models;

namespace Sequences.Web
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Look for any products.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }
            Random random = new Random();
            int numberOfProducts = random.Next(1, 51);
            var products = Enumerable.Range(1, numberOfProducts)
                .Select(i => new Product
                {
                    Name = $"Product #{i}",
                    Price = CalculatePrice(i)
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        private static decimal CalculatePrice(int index)
        {
            // Example: Price increases exponentially based on the index.
            return ((index * index)%50); // Example: Price = index squared
                                  // Or:
                                  // return index * 5;  // Example: Price = index * 5
                                  // Or: return a fixed price;
                                  // return 100
        }
    }
}
