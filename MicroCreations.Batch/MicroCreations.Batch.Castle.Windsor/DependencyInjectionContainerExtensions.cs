using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MicroCreations.Batch.Builders;
using MicroCreations.Batch.Context;
using MicroCreations.Batch.Operations;
using MicroCreations.Batch.Processors;

namespace MicroCreations.Batch.Castle.Windsor
{
    public static class DependencyInjectionContainerExtensions
    {
        public static void RegisterBatchAggregator(this IWindsorContainer container)
        {
            var assemblyDescriptor = Classes.FromAssemblyContaining<IBatchAggregator>();
            
            container.Register(assemblyDescriptor.BasedOn<IProcessor>().LifestyleSingleton());
            container.Register(Component.For<IBatchAggregator>().ImplementedBy<BatchAggregator>().LifestyleSingleton());

            var codeBaseAssemblyFilter = new AssemblyFilter(Assembly.GetExecutingAssembly().CodeBase);
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn<IOperationExecutor>().LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn<IContextBuilder>().LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn(typeof(IRequestBuilder<>)).LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn(typeof(IRequestBuilderFactory<>)).LifestyleSingleton());
        }
    }
}
