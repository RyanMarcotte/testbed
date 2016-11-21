using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.MicroKernel;
using CQSDIContainer.Attributes;
using CQSDIContainer.Contributors.Interfaces;
using CQSDIContainer.Interceptors;
using CQSDIContainer.Interceptors.Attributes;
using CQSDIContainer.Utilities;

namespace CQSDIContainer.Contributors
{
	public enum InterceptorUsageOptions
	{
		/// <summary>
		/// The interceptor will apply to no handlers.
		/// </summary>
		None,

		/// <summary>
		/// The interceptor will only apply to query handlers.
		/// </summary>
		QueryHandlersOnly,

		/// <summary>
		/// The interceptor will only apply to command handlers.
		/// </summary>
		CommandHandlersOnly,

		/// <summary>
		/// The interceptor will apply to any handler.
		/// </summary>
		AllHandlers
	}

	/// <summary>
	/// Base class for component model construction contributors used to apply interceptors to CQS handlers.
	/// </summary>
	/// <typeparam name="TInterceptorType">The CQS interceptor type.</typeparam>
	public abstract class CQSInterceptorContributor<TInterceptorType> : ICQSInterceptorContributor
		where TInterceptorType : CQSInterceptor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CQSInterceptorContributor{TInterceptorType}"/> class.
		/// </summary>
		/// <param name="isContributingToComponentModelConstructionForNestedCQSHandlers">Indicates if the contributor is managing the application of interceptors to nested CQS handlers.</param>
		/// <remarks>
		/// A handler is 'nested' if it is injected into another handler as a dependency.
		/// </remarks>
		protected CQSInterceptorContributor(bool isContributingToComponentModelConstructionForNestedCQSHandlers)
		{
			IsContributingToComponentModelConstructionForNestedCQSHandlers = isContributingToComponentModelConstructionForNestedCQSHandlers;
		}

		/// <summary>
		/// Indicates if the contributor is managing the application of interceptors to nested CQS handlers.
		/// </summary>
		public bool IsContributingToComponentModelConstructionForNestedCQSHandlers { get; }

		/// <summary>
		/// Indicates which types of handlers to apply the interceptor to.
		/// </summary>
		public abstract InterceptorUsageOptions HandlerTypesToApplyTo { get; }

		/// <summary>
		/// Indicates if the interceptor should be applied to the component model corresponding to a CQS handler.
		/// </summary>
		/// <param name="kernel">The IoC container.</param>
		/// <param name="model">The component model for the CQS handler.</param>
		/// <returns></returns>
		/// <remarks>
		/// Use this method to apply opt-in or opt-out logic.
		/// </remarks>
		protected abstract bool ShouldApplyInterceptor(IKernel kernel, ComponentModel model);

		/// <summary>
		/// Apply the interceptor to the component model.
		/// </summary>
		/// <param name="kernel">The IoC kernel.</param>
		/// <param name="model">The component model.</param>
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			switch (HandlerTypesToApplyTo)
			{
				case InterceptorUsageOptions.None:
					return;
					
				case InterceptorUsageOptions.QueryHandlersOnly:
					if (!CQSHandlerTypeCheckingUtility.IsQueryHandler(model.Implementation))
						return;
					break;

				case InterceptorUsageOptions.CommandHandlersOnly:
					if (!CQSHandlerTypeCheckingUtility.IsCommandHandler(model.Implementation))
						return;
					break;

				case InterceptorUsageOptions.AllHandlers:
					if (!CQSHandlerTypeCheckingUtility.IsCQSHandler(model.Implementation))
						return;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var shouldApplyInterceptor = ShouldApplyInterceptor(kernel, model);
			var shouldApplyToNestedHandler = typeof(TInterceptorType).GetCustomAttribute<ApplyToNestedHandlersAttribute>() != null;
			if (!shouldApplyInterceptor || (IsContributingToComponentModelConstructionForNestedCQSHandlers && !shouldApplyToNestedHandler))
				return;

			var interceptorReference = InterceptorReference.ForType<TInterceptorType>();
			var interceptorIsAlreadyApplied = model.Interceptors.Any(x => x.Equals(interceptorReference));
			if (!interceptorIsAlreadyApplied)
				model.Interceptors.Add(interceptorReference);
		}
	}
}
