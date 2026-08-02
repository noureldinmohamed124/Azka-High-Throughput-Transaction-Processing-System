using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Azka_Transaction_Processing_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Presistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.HasIndex(x => x.Phone)
                .IsUnique();

            builder.HasData(
                new Customer { Id = 1, FullName = "Omar Youssef", Email = "omar@gamil.com", Phone = "01064821657" },
                new Customer { Id = 2, FullName = "Ahmed Mohamed", Email = "ahmed@gamil.com", Phone = "01067521657" },
                new Customer { Id = 3, FullName = "Mohamed Hossam", Email = "mohamed@gamil.com", Phone = "01035821657" },
                new Customer { Id = 4, FullName = "Adel Mostafa", Email = "adel@gamil.com", Phone = "01464821957" }
            );
        }
    }
}
