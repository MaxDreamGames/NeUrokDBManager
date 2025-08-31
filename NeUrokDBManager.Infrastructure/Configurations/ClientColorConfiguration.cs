using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeUrokDBManager.Core.Entities;

namespace NeUrokDBManager.Infrastructure.Configurations
{
    public class ClientColorConfiguration : IEntityTypeConfiguration<ClientColor>
    {
        public void Configure(EntityTypeBuilder<ClientColor> builder)
        {
            builder.HasKey(x => x.ClientId); // 1 к 1, ClientId = PK и FK

            builder
                .Property(x => x.Color)
                .HasConversion(
                    c => c.ToArgb(),
                    s => Color.FromArgb(s)
                )
                .HasMaxLength(9);

            builder.HasOne(x => x.Client)
                .WithOne(c => c.ClientColor)
                .HasForeignKey<ClientColor>(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ClientColors");
        }
    }
}
