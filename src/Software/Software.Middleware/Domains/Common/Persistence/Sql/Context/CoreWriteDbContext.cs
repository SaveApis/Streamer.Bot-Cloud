using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Persistence.Sql.Configurations;
using Utils.EntityFrameworkCore.Infrastructure.Context.Base;

namespace Software.Middleware.Domains.Common.Persistence.Sql.Context;

public class CoreWriteDbContext(DbContextOptions<CoreWriteDbContext> options) : BaseWriteDbContext<CoreWriteDbContext>(options)
{
    protected override string Schema => "Core";

    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();
    public DbSet<ApplicationScopeEntity> ApplicationScopes => Set<ApplicationScopeEntity>();

    protected override void RegisterConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationScopeEntityTypeConfiguration());
    }
}
