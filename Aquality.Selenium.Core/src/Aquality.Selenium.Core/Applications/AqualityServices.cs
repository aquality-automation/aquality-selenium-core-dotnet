﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Aquality.Selenium.Core.Applications
{
    public abstract class AqualityServices<TApplication>
        where TApplication : class, IApplication
    {
        private static readonly ThreadLocal<TApplication> AppContainer = new ThreadLocal<TApplication>();
        private static readonly ThreadLocal<IServiceProvider> ServiceProviderContainer = new ThreadLocal<IServiceProvider>();

        protected AqualityServices()
        {
        }

        protected static bool IsApplicationStarted()
        {
            return AppContainer.IsValueCreated && AppContainer.Value.IsStarted;
        }
        
        protected static TApplication GetApplication(Func<IServiceProvider, TApplication> startApplicationFunction, Func<IServiceCollection> serviceCollectionProvider = null)
        {
            if (!IsApplicationStarted())
            {
                AppContainer.Value = startApplicationFunction(
                    GetServiceProvider(service => GetApplication(startApplicationFunction, serviceCollectionProvider), serviceCollectionProvider));
            }
            return AppContainer.Value;
        }

        protected static void SetApplication(TApplication application)
        {
            AppContainer.Value = application;
        }

        protected static IServiceProvider GetServiceProvider(Func<IServiceProvider, TApplication> applicationSupplier, Func<IServiceCollection> serviceCollectionProvider = null)
        {
            if (!ServiceProviderContainer.IsValueCreated)
            {
                IServiceCollection services;
                if (serviceCollectionProvider == null)
                {
                    services = new ServiceCollection();
                    new Startup().ConfigureServices(services, applicationSupplier);
                }
                else
                {
                    services = serviceCollectionProvider();
                }
                ServiceProviderContainer.Value = services.BuildServiceProvider();
            }
            return ServiceProviderContainer.Value;
        }

        protected static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProviderContainer.Value = serviceProvider;
        }
    }
}
