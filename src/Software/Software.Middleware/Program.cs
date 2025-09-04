using System.Reflection;
using Software.Core.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSaveApis(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
