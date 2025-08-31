using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeUrokDBManager.Core.Entities;

namespace NeUrokDBManager.Infrastructure.Configurations
{
    public class UpdateConfiguration : IEntityTypeConfiguration<Update>
    {
        public void Configure(EntityTypeBuilder<Update> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UpdateingDate)
                .IsRequired();

            builder.HasIndex(x => x.UpdateingDate)
                .IsUnique();
        }
    }
}
