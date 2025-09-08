using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;

namespace Utils.Hangfire.Infrastructure.Attributes;

public class HangfireJobName(string displayName) : JobDisplayNameAttribute(displayName)
{
    public override string Format(DashboardContext context, Job job)
    {
        var formatted = base.Format(context, job);

        var prefix = ReadPrefix(job.Type.Namespace, "Domains");
        prefix ??= ReadPrefix(job.Type.Namespace, "Integrations");
        prefix ??= "Core";

        return $"[{prefix}] {formatted}";
    }

    private static string? ReadPrefix(string? ns, string part)
    {
        if (string.IsNullOrWhiteSpace(ns))
        {
            return null;
        }

        var parts = ns.Split('.');
        var index = Array.IndexOf(parts, part);

        if (index >= 0 && index < parts.Length - 1)
        {
            return parts[index + 1];
        }

        return null;
    }
}
