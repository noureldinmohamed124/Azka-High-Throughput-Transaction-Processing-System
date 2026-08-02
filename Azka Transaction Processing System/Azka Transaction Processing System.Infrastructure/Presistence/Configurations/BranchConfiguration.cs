using Azka_Transaction_Processing_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Presistence.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasData(
                new Branch { Id = 1, Name = "Cairo", Code = "C1" },
                new Branch { Id = 2, Name = "Alex", Code = "A1" },
                new Branch { Id = 3, Name = "Giza", Code = "G1" },
                new Branch { Id = 4, Name = "Suez", Code = "S1" }
            );
        }
    }
}
