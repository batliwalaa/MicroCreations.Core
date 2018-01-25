using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MicroCreations.Batch.Common.DependencyGraph;

namespace MicroCreations.Batch.DependencyGraph
{
    public static class DependencyInjectionContainerExtensions
    {
        public static void RegisterDependencyGraph(this IWindsorContainer container)
        {
            container.Register(Component.For<IDependencyGraphBuilder>().ImplementedBy<DependencyGraphBuilder>().LifestyleSingleton());
            container.Register(Component.For<IDependencyGraph>().ImplementedBy<DefaultDependencyGraph>().LifestyleSingleton());
        }

        public static IDependencyGraphBuilder DependencyGraphBuilder(this IWindsorContainer container)
        {
            return container.Resolve<IDependencyGraphBuilder>();
        }
    }
}
