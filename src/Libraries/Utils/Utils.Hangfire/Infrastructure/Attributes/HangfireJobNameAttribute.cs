using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;

namespace Utils.Hangfire.Infrastructure.Attributes;

public class HangfireJobNameAttribute(string displayName) : JobDisplayNameAttribute(displayName)
{
    public override string Format(DashboardContext context, Job job)
    {
        var formatted = base.Format(context, job);

        var prefix = ReadPrefix(job.Type.Namespace, "Domains");
        prefix ??= ReadPrefix(job.Type.Namespace, "Integrations");
        prefix ??= "Core";

        return $"[{prefix}] {formatted}";
    }

    private static string? ReadPrefix(string? @namespace, string marker)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            return null;
        }

        var parts = @namespace.Split('.');

        var markerIndex = Array.IndexOf(parts, marker);
        if (markerIndex < 0 || markerIndex + 1 >= parts.Length)
        {
            return null;
        }

        return parts[markerIndex + 1];
    }
}
