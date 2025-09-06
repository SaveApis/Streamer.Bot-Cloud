using System.Reflection;
using Correlate.AspNetCore;
using Software.Core.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSaveApis(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseCorrelate();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
