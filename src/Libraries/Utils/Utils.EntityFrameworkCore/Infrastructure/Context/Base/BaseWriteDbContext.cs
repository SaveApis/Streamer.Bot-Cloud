using Microsoft.EntityFrameworkCore;

namespace Utils.EntityFrameworkCore.Infrastructure.Context.Base;

public abstract class BaseWriteDbContext<TContext>(DbContextOptions<TContext> options) : BaseDbContext<TContext>(options), IWriteDbContext where TContext : DbContext;
