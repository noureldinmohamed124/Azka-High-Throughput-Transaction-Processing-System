using Azka_Transaction_Processing_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Presistance.Configurations
{
    public class ReceiptSequenceConfiguration : IEntityTypeConfiguration<ReceiptSequence>
    {
        public void Configure(EntityTypeBuilder<ReceiptSequence> builder)
        {
            builder.ToTable("ReceiptSequences");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Prefix)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.LastSequence)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.Prefix,
                x.Date
            })
            .IsUnique();

        }
    }
}
