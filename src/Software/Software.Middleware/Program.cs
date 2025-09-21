using System.Reflection;
using Autofac;
using Correlate.AspNetCore;
using Software.Core.Infrastructure.Extensions;
using Software.Middleware.Domains.Application;
using Software.Middleware.Domains.Application.Application.Backend.Authorization;
using Software.Middleware.Domains.Common.Persistence.Sql.Context;
using Utils.EntityFrameworkCore.Infrastructure.Extensions;
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

await app.UseCompactHangfireDashboard().ConfigureAwait(false);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
