using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Aquality.Selenium.Core.Applications
{
    public abstract class AqualityServices<TApplication>
        where TApplication : class, IApplication
    {
        private static readonly AsyncLocal<TApplication> AppContainer = new AsyncLocal<TApplication>();
        private static readonly AsyncLocal<IServiceProvider> ServiceProviderContainer = new AsyncLocal<IServiceProvider>();

        protected AqualityServices()
        {
        }

        protected static bool IsApplicationStarted()
        {
            return AppContainer.Value != null && AppContainer.Value.IsStarted;
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
            if (ServiceProviderContainer.Value == null)
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
