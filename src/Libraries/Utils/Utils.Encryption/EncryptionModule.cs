using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.Encryption.Domain.Options;

namespace Utils.Encryption;

public class EncryptionModule(IConfiguration configuration) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.Configure<EncryptionOption>(configuration.GetSection("Encryption"));

        builder.Populate(collection);
    }
}
