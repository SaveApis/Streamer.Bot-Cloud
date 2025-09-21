using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Software.Middleware.Domains.Application.Domain.Models.Entities;

namespace Software.Middleware.Domains.Application.Persistence.Sql.Configurations;

public class ApplicationEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("Applications");
        builder.HasKey(x => x.Key);

        builder.Property(x => x.Key).HasMaxLength(100).IsRequired().ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Source).IsRequired().HasConversion<string>();
        builder.Property(x => x.AuthId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.AuthSecret).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Iv).HasMaxLength(100).IsRequired();

        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasIndex(x => x.Source);
        builder.HasIndex(x => x.AuthId).IsUnique();
        builder.HasIndex(x => x.AuthSecret).IsUnique();
        builder.HasIndex(x => x.Iv).IsUnique();

        builder.HasMany(x => x.Scopes).WithMany();
    }
}
