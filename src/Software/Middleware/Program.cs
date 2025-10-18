using System.Reflection;
using Software.Core.Application.Extensions;
using Utils.Rest.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseAutofac(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseLoggingMiddleware();
app.UseExceptionMiddleware();

app.MapControllers();

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
