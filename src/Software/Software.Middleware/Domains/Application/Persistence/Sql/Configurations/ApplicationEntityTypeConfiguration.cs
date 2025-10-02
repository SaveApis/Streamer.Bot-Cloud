using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Software.Middleware.Domains.Application.Domain.Models.Entities;

namespace Software.Middleware.Domains.Application.Persistence.Sql.Configurations;

public class ApplicationEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("Applications");
        builder.HasKey(e => e.Key);

        builder.Property(e => e.Key).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired(false);
        builder.Property(e => e.State).IsRequired().HasConversion<string>();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).IsRequired(false).HasMaxLength(500);
        builder.Property(e => e.AuthId).IsRequired().HasMaxLength(100);
        builder.Property(e => e.AuthSecret).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Iv).IsRequired().HasMaxLength(100);

        builder.HasIndex(e => e.State);
        builder.HasIndex(e => e.AuthId).IsUnique();
        builder.HasIndex(e => e.AuthSecret).IsUnique();
        builder.HasIndex(e => e.Iv).IsUnique();

        builder.HasMany(e => e.Scopes)
            .WithMany()
            .UsingEntity(typeBuilder => typeBuilder.ToTable("ApplicationApplicationScopes"));
    }
}
