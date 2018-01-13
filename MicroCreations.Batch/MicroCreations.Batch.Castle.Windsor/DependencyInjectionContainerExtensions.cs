using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MicroCreations.Batch.Builders;
using MicroCreations.Batch.Common.Context;
using MicroCreations.Batch.Common.Operations;
using MicroCreations.Batch.DependencyGraph;
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
            container.Register(Component.For<IDependencyGraphBuilder>().ImplementedBy<DependencyGraphBuilder>().LifestyleSingleton());
            container.Register(Component.For<IDependencyGraph>().ImplementedBy<DefaultDependencyGraph>().LifestyleSingleton());

            var codeBaseAssemblyFilter = new AssemblyFilter(Assembly.GetExecutingAssembly().CodeBase);
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn<IOperationExecutor>().LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn<IContextBuilder>().LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn(typeof(IRequestBuilder<>)).LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn(typeof(IRequestBuilderFactory<>)).LifestyleSingleton());
            container.Register(Classes.FromAssemblyInDirectory(codeBaseAssemblyFilter).BasedOn<IDependencyOperationPredicate>().LifestyleSingleton());
        }

        public static IDependencyGraphBuilder DependencyGraphBuilder(this IWindsorContainer container)
        {
            return container.Resolve<IDependencyGraphBuilder>();
        }
    }
}
