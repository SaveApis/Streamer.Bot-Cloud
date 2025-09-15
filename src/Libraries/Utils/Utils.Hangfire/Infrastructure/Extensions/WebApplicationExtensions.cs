using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utils.Hangfire.Domain.Options;

namespace Utils.Hangfire.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseCompactHangfireDashboard(this WebApplication application)
    {
        await using var scope = application.Services.CreateAsyncScope();
        var monitor = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<HangfireOption>>();
        var option = monitor.CurrentValue;

        var appPath = string.IsNullOrWhiteSpace(option.Dashboard.AppPath) ? "/hangfire" : option.Dashboard.AppPath;
        var prefixPath = string.IsNullOrWhiteSpace(option.Dashboard.PrefixPath)
            ? string.Empty
            : option.Dashboard.PrefixPath;
        var dashboardTitle = string.IsNullOrWhiteSpace(option.Dashboard.DashboardTitle)
            ? "Hangfire Dashboard"
            : option.Dashboard.DashboardTitle;

        var options = new DashboardOptions
        {
            PrefixPath = prefixPath,
            AppPath = appPath,
            DarkModeEnabled = true,
            DashboardTitle = dashboardTitle,
            DefaultRecordsPerPage = 500,
            DisplayStorageConnectionString = true,
            Authorization = [], //TODO: Implement authorization
            AsyncAuthorization = [], //TODO: Implement authorization
        };
        application.UseHangfireDashboard(appPath, options);
    }
}
