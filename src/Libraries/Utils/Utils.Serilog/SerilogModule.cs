using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Utils.Serilog.Domain.Options;

namespace Utils.Serilog;

public class SerilogModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();
        collection.Configure<LoggingOption>(configuration.GetSection("Logging"));

        collection.AddSerilog((provider, loggerConfiguration) =>
        {
            var environment = provider.GetRequiredService<IHostEnvironment>();

            var monitor = provider.GetRequiredService<IOptionsMonitor<LoggingOption>>();
            var loggingOptions = monitor.CurrentValue;
            var betterStackOptions = loggingOptions.BetterStack;

            loggerConfiguration.MinimumLevel.Verbose();
            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithProperty("Application", loggingOptions.ApplicationName);
            loggerConfiguration.MinimumLevel.Override("Hangfire", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Microsoft.Extensions.Hosting", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Correlate.AspNetCore", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
            loggerConfiguration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            if (!environment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.BetterStack(betterStackOptions.Token, $"https://{betterStackOptions.Host}");
            }
        });

        builder.Populate(collection);
    }
}
