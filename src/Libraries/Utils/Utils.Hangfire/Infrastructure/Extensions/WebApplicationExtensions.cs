using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Utils.Hangfire.Application.Handler.Authorization;
using Utils.Hangfire.Domain.Options;

namespace Utils.Hangfire.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseDefaultHangfireDashboard(this WebApplication application)
    {
        var scope = application.Services.CreateAsyncScope();
        await using var _ = scope.ConfigureAwait(false);

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var hangfireOption = configuration.GetSection("Hangfire").Get<HangfireOption>() ?? throw new InvalidOperationException("Hangfire configuration is missing.");
        var dashboardOption = new DashboardOptions
        {
            PrefixPath = hangfireOption.Dashboard.PrefixPath,
            AppPath = null,
            AsyncAuthorization = application.Environment.IsDevelopment() 
                ? []
                : [scope.ServiceProvider.GetRequiredService<DefaultDashboardAuthorizationFilter>()],
            DarkModeEnabled = true,
            DashboardTitle = hangfireOption.Dashboard.HangfireDashboardTitle,
            DefaultRecordsPerPage = 500,
            DisplayStorageConnectionString = true,
        };

        application.UseHangfireDashboard("/hangfire", dashboardOption);
    }
}
