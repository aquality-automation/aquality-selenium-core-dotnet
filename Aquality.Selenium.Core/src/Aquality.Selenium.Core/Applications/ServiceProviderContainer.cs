using System;
using System.Threading;

namespace Aquality.Selenium.Core.Applications
{
    public class ServiceProviderContainer
    {
        protected static readonly ThreadLocal<IServiceProvider> ServiceProviderInstanceHolder = new ThreadLocal<IServiceProvider>();

        protected ServiceProviderContainer()
        {
        }
    }
}
