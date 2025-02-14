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
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(p => p.KeyId);
            builder.Property(p => p.KeyId).HasColumnName("Id");
            builder.HasAlternateKey(o => o.NationalCode).HasName("AlternateKey_NationalCode");
            builder.HasIndex(o => o.PhoneNumber).IsUnique();
            //builder.HasIndex(o => new { o.PhoneNumber, o.NationalCode });  // multiple index
            //builder.HasKey(p => new { p.KeyId1, p.KeyId2 });  // multiple keys
        }
    }
}
