using System.Diagnostics.CodeAnalysis;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MicroCreations.Batch.Context;
using MicroCreations.Batch.Processors;

namespace MicroCreations.Batch.Castle.Windsor
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionContainerExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void RegisterBatchAggregator(this IWindsorContainer container, bool registerDefaultContextBuilder = true)
        {
            var assemblyDescriptor = Classes.FromAssemblyContaining<IBatchAggregator>();
            
            container.Register(assemblyDescriptor.BasedOn<IProcessor>().LifestyleSingleton());
            container.Register(Component.For<IBatchAggregator>().ImplementedBy<BatchAggregator>().LifestyleSingleton());
            
            if (registerDefaultContextBuilder)
            {
                container.Register(Component.For<IContextBuilder>().ImplementedBy<DefaultContextBuilder>().LifestyleSingleton().Named("DefaultContextBuilder"));
            }
        }
    }
}
