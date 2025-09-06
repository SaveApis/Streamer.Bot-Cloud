using Autofac;
using Autofac.Extensions.DependencyInjection;
using Correlate.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Utils.Correlation;

public class CorrelationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.AddCorrelate(options => options.IncludeInResponse = true);
        collection.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.CorrelateRequests());
        
        builder.Populate(collection);
    }
}
