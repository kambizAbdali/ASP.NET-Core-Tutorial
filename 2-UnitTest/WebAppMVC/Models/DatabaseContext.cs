using Microsoft.EntityFrameworkCore;
using WebAppMVC.Models.Entites;

namespace WebAppMVC.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
