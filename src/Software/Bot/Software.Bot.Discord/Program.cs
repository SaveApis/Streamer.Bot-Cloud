using System.Reflection;
using Correlate.AspNetCore;
using Software.Core.Infrastructure.Extensions;
using Utils.Hangfire.Domain.Types;
using Utils.Hangfire.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.ConfigureSaveApis(Assembly.GetExecutingAssembly(), out var applicationType);

var app = builder.Build();

if (applicationType == ApplicationType.Backend)
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SupportedSubmitMethods());

    await app.UseDefaultHangfireDashboard().ConfigureAwait(false);

    app.UseCorrelate();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
