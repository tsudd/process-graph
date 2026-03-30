using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Users;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");


        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName).HasMaxLength(100)
            .HasConversion(firstName => firstName.Value, value => new FirstName(value));

        builder.Property(x => x.LastName).HasMaxLength(100)
            .HasConversion(lastName => lastName.Value, value => new LastName(value));

        builder.Property(x => x.Email).HasMaxLength(300).HasConversion(email => email.Value, value => new Email(value));
        builder.HasIndex(x => x.Email).IsUnique();
    }
}