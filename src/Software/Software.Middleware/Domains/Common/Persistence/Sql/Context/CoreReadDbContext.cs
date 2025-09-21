using Microsoft.EntityFrameworkCore;
using Software.Middleware.Domains.Application.Domain.Models.Entities;
using Software.Middleware.Domains.Application.Persistence.Sql.Configurations;
using Utils.EntityFrameworkCore.Infrastructure.Context.Base;

namespace Software.Middleware.Domains.Common.Persistence.Sql.Context;

public class CoreReadDbContext(DbContextOptions<CoreReadDbContext> options) : BaseReadDbContext<CoreReadDbContext>(options)
{
    protected override string Schema => "Core";

    public IQueryable<ApplicationEntity> Applications => Set<ApplicationEntity>().AsNoTracking();
    public IQueryable<ApplicationScopeEntity> ApplicationScopes => Set<ApplicationScopeEntity>().AsNoTracking();

    protected override void RegisterConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationScopeEntityTypeConfiguration());
    }
}
