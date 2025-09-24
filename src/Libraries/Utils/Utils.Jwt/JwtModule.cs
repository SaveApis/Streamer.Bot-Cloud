using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Utils.Jwt.Application.Builders;
using Utils.Jwt.Domain.Options;
using Utils.Jwt.Infrastructure;

namespace Utils.Jwt;

public class JwtModule(IConfiguration configuration, IHostEnvironment environment) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();
        var option = configuration.GetSection("Jwt").Get<JwtOption>();
        if (option == null)
        {
            throw new InvalidOperationException("Jwt configuration section is missing or invalid.");
        }

        collection.Configure<JwtOption>(configuration.GetSection("Jwt"));

        collection.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var signingKeyBytes = Convert.FromBase64String(option.SigningKey);

                    options.Audience = option.Audience;
                    options.RequireHttpsMetadata = !environment.IsDevelopment();
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = option.Issuer,
                        ValidAudience = option.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                    };
                }
            );

        builder.Populate(collection);

        builder.RegisterType<JwtBuilder>().As<IJwtBuilder>();
    }
}
