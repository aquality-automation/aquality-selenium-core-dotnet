using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Aquality.Selenium.Core.Applications
{
    public abstract class ApplicationManager<TApplication>
        where TApplication : class, IApplication
    {
        private static readonly ThreadLocal<TApplication> AppContainer = new ThreadLocal<TApplication>();        

        protected ApplicationManager()
        {
        }

        public static bool IsApplicationStarted()
        {
            return AppContainer.IsValueCreated && AppContainer.Value.Driver.SessionId != null;
        }
        
        protected static TApplication GetApplication(Func<IServiceProvider, TApplication> applicationSupplier, Func<IServiceCollection> serviceCollectionProvider = null)
        {
            if (!IsApplicationStarted())
            {
                AppContainer.Value = applicationSupplier(Startup.ConfigureServiceProvider(service => GetApplication(applicationSupplier, serviceCollectionProvider), serviceCollectionProvider));
            }
            return AppContainer.Value;
        }

        protected static void SetApplication(TApplication application)
        {
            AppContainer.Value = application;
        }        
    }
}
