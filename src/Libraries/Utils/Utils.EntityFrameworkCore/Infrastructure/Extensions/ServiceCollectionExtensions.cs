using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utils.EntityFrameworkCore.Application.Extensions;
using Utils.EntityFrameworkCore.Domain.Options;
using Utils.EntityFrameworkCore.Infrastructure.Context;

namespace Utils.EntityFrameworkCore.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterEntityFrameworkCore<TReadDbContext, TWriteDbContext>(this IServiceCollection services)
        where TReadDbContext : DbContext, IReadDbContext
        where TWriteDbContext : DbContext, IWriteDbContext
    {
        if (EF.IsDesignTime)
        {
            const string connectionString = "Server=localhost;Port=3306;";
            services.AddDbContext<TWriteDbContext>(options => options.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion, builder => builder.RegisterDefaultSettings()));
        }
        else
        {
            services.AddPooledDbContextFactory<TReadDbContext>((provider, builder) =>
                {
                    var monitor = provider.GetRequiredService<IOptionsMonitor<MySqlOption>>();
                    var option = monitor.CurrentValue;

                    builder.UseMySql(option.ToString(), ServerVersion.AutoDetect(option.ToString()), b => b.RegisterDefaultSettings());
                }
            );
            services.AddPooledDbContextFactory<TWriteDbContext>((provider, builder) =>
                {
                    var monitor = provider.GetRequiredService<IOptionsMonitor<MySqlOption>>();
                    var option = monitor.CurrentValue;

                    builder.UseMySql(option.ToString(), ServerVersion.AutoDetect(option.ToString()), b => b.RegisterDefaultSettings());
                }
            );
        }

    }
}
