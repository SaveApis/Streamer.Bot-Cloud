using Microsoft.EntityFrameworkCore;

namespace Utils.EntityFrameworkCore.Infrastructure.Context.Base;

public abstract class BaseDbContext<TContext>(DbContextOptions<TContext> options) : DbContext(options) where TContext : DbContext
{
    protected abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        RegisterConfigurations(modelBuilder);
    }

    protected virtual void RegisterConfigurations(ModelBuilder modelBuilder)
    {
    }
}
