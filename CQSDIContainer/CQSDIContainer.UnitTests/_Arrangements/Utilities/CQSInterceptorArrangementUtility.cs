﻿using System;
using System.Linq;
using CQSDIContainer.UnitTests.Customizations.Utilities;
using Ploeh.AutoFixture;

// ReSharper disable once CheckNamespace
namespace CQSDIContainer.UnitTests.Arrangements.Utilities
{
	public class CQSInterceptorArrangementUtility
	{
		/// <summary>
		/// Create a <see cref="CQSInterceptorWithExceptionHandlingCustomizationBase{TInterceptorType}"/> instance.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static ICustomization CreateCQSInterceptorCustomizationInstance(Type type)
		{
			// does the type implement the ICQSInterceptorWithExceptionHandlingCustomization<> interface?
			var genericInterface = type.GetInterfaces();
			if (genericInterface.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICQSInterceptorWithExceptionHandlingCustomization<>)) == null)
				throw new NotCQSInterceptorWithExceptionHandlingCustomizationBaseClassTypeException(type);

			return (ICustomization)Activator.CreateInstance(type);
		}
	}

	public class NotCQSInterceptorWithExceptionHandlingCustomizationBaseClassTypeException : Exception
	{
		public NotCQSInterceptorWithExceptionHandlingCustomizationBaseClassTypeException(Type offendingType)
			: base($"Expected type inheriting from '{typeof(CQSInterceptorWithExceptionHandlingCustomizationBase<>)}'!!  Received '{offendingType}'.")
		{
			
		}
	}
}
