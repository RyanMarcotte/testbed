using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.SubResolvers
{
	public class NestedCQSHandlerResolver : ISubDependencyResolver
	{
		private readonly IKernel _kernel;

		public NestedCQSHandlerResolver(IKernel kernel)
		{
			_kernel = kernel;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return CQSHandlerTypeCheckingUtility.IsCQSHandler(model.Implementation) && CQSHandlerTypeCheckingUtility.IsCQSHandler(dependency.TargetItemType);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
		{
			return _kernel.Resolve(dependency.TargetItemType);
		}
	}
}