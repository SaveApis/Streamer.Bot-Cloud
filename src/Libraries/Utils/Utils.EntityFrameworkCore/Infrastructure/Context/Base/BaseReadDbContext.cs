using Microsoft.EntityFrameworkCore;

namespace Utils.EntityFrameworkCore.Infrastructure.Context.Base;

public abstract class BaseReadDbContext<TContext>(DbContextOptions<TContext> options) : BaseDbContext<TContext>(options), IReadDbContext where TContext : DbContext;
