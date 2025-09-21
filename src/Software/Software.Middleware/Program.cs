using System.Reflection;
using Correlate.AspNetCore;
using Software.Core.Infrastructure.Extensions;
using Utils.Hangfire.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddScheme<ApplicationBasicAuthHandlerOption, ApplicationBasicAuthHandler>(ApplicationBasicAuthHandler.SchemeName, null);

builder.ConfigureSaveApis(Assembly.GetExecutingAssembly(), (context, containerBuilder, assemblyHelper) => containerBuilder.RegisterModule(new ApplicationModule(context.Configuration)));
builder.Services.RegisterEntityFrameworkCore<CoreReadDbContext, CoreWriteDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => options.SupportedSubmitMethods());

app.UseCorrelate();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
await app.UseCompactHangfireDashboard().ConfigureAwait(false);

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
