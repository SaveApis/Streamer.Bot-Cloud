using System.Reflection;
using Software.Core.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseAutofac(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
