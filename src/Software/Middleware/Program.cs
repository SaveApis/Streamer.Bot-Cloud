using System.Reflection;
using Software.Core.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseAutofac(Assembly.GetExecutingAssembly());

var app = builder.Build();

await app.StartAsync().ConfigureAwait(false);
await app.WaitForShutdownAsync().ConfigureAwait(false);
