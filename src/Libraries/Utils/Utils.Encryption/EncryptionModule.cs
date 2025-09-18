using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.Encryption.Domain.Options;
using Utils.Encryption.Infrastructure.Services.Encryption;
using Utils.Encryption.Infrastructure.Services.Hashing;

namespace Utils.Encryption;

public class EncryptionModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterOptions(builder, configuration);

        builder.RegisterType<EncryptionService>()
            .As<IEncryptionService>()
            .InstancePerLifetimeScope();
        builder.RegisterType<HashingService>()
            .As<IHashingService>()
            .InstancePerLifetimeScope();
    }

    private static void RegisterOptions(ContainerBuilder builder, IConfiguration configuration)
    {
        var collection = new ServiceCollection();

        collection.Configure<EncryptionOption>(configuration.GetSection("Encryption"));

        builder.Populate(collection);
    }
}
