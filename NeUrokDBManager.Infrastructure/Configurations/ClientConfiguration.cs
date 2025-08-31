using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeUrokDBManager.Core.Entities;

namespace NeUrokDBManager.Infrastructure.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

            builder.Property(x => x.StudentName)
                .IsRequired();

            builder.Property(x => x.Birthday)
                .IsRequired(false);

            builder.Property(x => x.RegistrationDate)
                .IsRequired();

            builder.Property(x => x.Class)
                .IsRequired();

            builder.Property(x => x.Courses)
                .IsRequired(false);

            builder.Property(x => x.ParentName)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .IsRequired();

            builder.Property(x => x.AnotherPhoneNumber)
                .IsRequired(false);

            builder.Property(x => x.Comments)
                .IsRequired(false);

        }
    }
}
