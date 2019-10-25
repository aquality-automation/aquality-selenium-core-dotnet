using System;
using System.Reflection;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium.Remote;

namespace Aquality.Selenium.Core.Tests.Applications
{
    public class ApplicationManagerTests
    {
        private const string SpecialSettingsFile = "special";
        private const string SpecialLanguageValue = "special";
        private static readonly TimeSpan SpecialTimeoutValue = TimeSpan.FromDays(1);

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices()
        {
            var application = ApplicationManager.Application;
            Assert.IsInstanceOf<CustomTimeoutConfiguration>(Startup.GetRequiredService<ITimeoutConfiguration>());
        }

        [Test]
        public void Should_BePossibleTo_GetCustomValues()
        {
            var application = ApplicationManager.Application;
            var timeoutConfiguration = Startup.GetRequiredService<ITimeoutConfiguration>() as CustomTimeoutConfiguration;
            Assert.AreEqual(SpecialTimeoutValue,  timeoutConfiguration.CustomTimeout);
        }

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices_WithCustomSettingsFile()
        {
            var application = ApplicationManager.Application;
            Assert.AreEqual(SpecialLanguageValue, Startup.GetRequiredService<ILoggerConfiguration>().Language);
        }

        private class ApplicationManager : ApplicationManager<IApplication>
        {
            public static IApplication Application => GetApplication(services => new Application(), () => RegisterServices(services => Application));

            private static IServiceCollection RegisterServices(Func<IServiceProvider, IApplication> applicationSupplier)
            {
                var settingsFile = new JsonSettingsFile($"Resources.settings.{SpecialSettingsFile}.json", Assembly.GetExecutingAssembly());
                var services = new ServiceCollection();
                Startup.ConfigureServices(services, applicationSupplier, settingsFile);
                services.AddSingleton<ITimeoutConfiguration>(new CustomTimeoutConfiguration(settingsFile));
                return services;
            }
        }

        private class CustomTimeoutConfiguration : TimeoutConfiguration
        {
            public CustomTimeoutConfiguration(ISettingsFile settingsFile) : base(settingsFile)
            {
                CustomTimeout = SpecialTimeoutValue;
            }

            public TimeSpan CustomTimeout { get; }
        }

        private class Application : IApplication
        {
            public RemoteWebDriver Driver => throw new NotImplementedException();

            public void SetImplicitWaitTimeout(TimeSpan timeout)
            {
                throw new NotImplementedException();
            }
        }
    }
}
