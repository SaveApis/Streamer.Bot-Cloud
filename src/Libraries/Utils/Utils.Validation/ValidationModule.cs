using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Utils.Core.Infrastructure.Helpers;
using Utils.Validation.Application.Services;
using Utils.Validation.Infrastructure.Services;

namespace Utils.Validation;

public class ValidationModule(IAssemblyHelper assemblyHelper) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var collection = new ServiceCollection();

        collection.AddValidatorsFromAssemblies(assemblyHelper.GetAssemblies().ToArray());

        builder.Populate(collection);

        builder.RegisterType<ValidationService>().As<IValidationService>().InstancePerLifetimeScope();
    }
}
