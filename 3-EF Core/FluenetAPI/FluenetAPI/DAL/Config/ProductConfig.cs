using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Config
{
    internal class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("tbl_Product", schema: "prod");
            builder.Property(o => o.Name).IsRequired();

            builder.Ignore(o => o.TempField); // it does not convert to field in Table

            builder.Property(o => o.Id).HasColumnName("ProductId");
            builder.Property(o => o.Name).HasColumnType("varchar(150)");
            builder.Property(o => o.Description).HasMaxLength(128);

        }
    }
}
