using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Aquality.Selenium.Core.Applications
{
    public abstract class ApplicationManager<TManager, TApplication> 
        where TManager : ApplicationManager<TManager, TApplication>
        where TApplication : class, IApplication
    {
        private static readonly ThreadLocal<TApplication> AppContainer = new ThreadLocal<TApplication>();
        private static readonly ThreadLocal<IServiceProvider> ServiceProviderContainer = new ThreadLocal<IServiceProvider>();

        public static bool IsStarted => AppContainer.IsValueCreated && AppContainer.Value.Driver.SessionId != null;
        
        protected static TApplication GetApplication(Func<IServiceProvider, TApplication> startApplicationFunction, IServiceCollection serviceCollection = null)
        {
            if (!IsStarted)
            {
                AppContainer.Value = startApplicationFunction(
                    GetServiceProvider(service => GetApplication(startApplicationFunction), serviceCollection));
            }
            return AppContainer.Value;
        }

        protected static IServiceProvider GetServiceProvider(Func<IServiceProvider, TApplication> applicationSupplier, IServiceCollection serviceCollection = null)
        {
            if (!ServiceProviderContainer.IsValueCreated)
            {
                var services = serviceCollection;
                if (services == null)
                {
                    services = new ServiceCollection();
                    new Startup().ConfigureServices(services, applicationSupplier);
                }
                ServiceProviderContainer.Value = services.BuildServiceProvider();
            }
            return ServiceProviderContainer.Value;
        }
    }
}
