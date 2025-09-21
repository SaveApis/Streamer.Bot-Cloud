using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Software.Middleware.Domains.Application.Domain.Models.Entities;

namespace Software.Middleware.Domains.Application.Persistence.Sql.Configurations;

public class ApplicationScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationScopeEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationScopeEntity> builder)
    {
        builder.ToTable("ApplicationScopes");
        builder.HasKey(e => e.Key);

        builder.Property(e => e.Key).IsRequired().HasMaxLength(100).ValueGeneratedNever();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
    }
}
