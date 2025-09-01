using Microsoft.EntityFrameworkCore;
using OwnedEntityTypesDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnedEntityTypesDemo.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().OwnsOne(o => o.WorkPlace);
            modelBuilder.Entity<User>().OwnsOne(o => o.Home);
            // پیکربندی Home به عنوان یک Owned Entity Type
            //modelBuilder.Entity<User>()
            //    .OwnsOne(u => u.Home, Home =>
            //    {
            //        Home.Property(a => a.Street).HasColumnName("HomeStreet");
            //        Home.Property(a => a.City).HasColumnName("HomeCity");
            //        Home.Property(a => a.PostalCode).HasColumnName("HomePostalCode");
            //        Home.Property(a => a.Country).HasColumnName("HomeCountry");
            //    });

            //// پیکربندی WorkPlace به عنوان یک Owned Entity Type
            //modelBuilder.Entity<User>()
            //    .OwnsOne(u => u.WorkPlace, WorkPlace =>
            //    {
            //        WorkPlace.Property(a => a.Street).HasColumnName("WorkPlaceStreet");
            //        WorkPlace.Property(a => a.City).HasColumnName("WorkPlaceCity");
            //        WorkPlace.Property(a => a.Country).HasColumnName("WorkPlaceCountry");
            //        WorkPlace.Property(a => a.PostalCode).HasColumnName("WorkPlacePostalCode");
            //    });
        }
    }
}