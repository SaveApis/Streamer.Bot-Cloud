using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Utils.Core.Application.Helpers;

namespace Utils.Rest;

public class RestModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        var mvcBuilder = collection.AddControllers();
        foreach (var assembly in assemblyHelper.GetAssemblies())
        {
            mvcBuilder.AddApplicationPart(assembly);
        }
        mvcBuilder.AddNewtonsoftJson(options =>
        {
            options.AllowInputFormatterExceptionMessages = true;
            options.ReadJsonWithRequestCulture = true;
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        builder.Populate(collection);
    }
}
