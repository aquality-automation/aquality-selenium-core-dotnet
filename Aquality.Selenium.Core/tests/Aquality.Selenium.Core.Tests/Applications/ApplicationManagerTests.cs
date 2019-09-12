using System;
using System.Reflection;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
            Assert.IsInstanceOf<CustomTimeoutConfiguration>(ApplicationManager.ServiceProvider.GetService<ITimeoutConfiguration>());
        }

        [Test]
        public void Should_BePossibleTo_GetCustomValues()
        {
            var timeoutConfiguration = ApplicationManager.ServiceProvider.GetService<ITimeoutConfiguration>() as CustomTimeoutConfiguration;
            Assert.AreEqual(SpecialTimeoutValue,  timeoutConfiguration.CustomTimeout);
        }

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices_WithCustomSettingsFile()
        {
            Assert.AreEqual(SpecialLanguageValue, ApplicationManager.ServiceProvider.GetService<ILoggerConfiguration>().Language);
        }


        private class ApplicationManager : ApplicationManager<ApplicationManager, IApplication>
        {
            public static IApplication Application => GetApplication(StartApplicationFunction, RegisterServices(StartApplicationFunction));

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application, RegisterServices(StartApplicationFunction));

            private static IServiceCollection RegisterServices(Func<IServiceProvider, IApplication> applicationSupplier)
            {
                var services = new ServiceCollection();
                var startup = new Startup();
                var settingsFile = new JsonFile($"Resources.settings.{SpecialSettingsFile}.json", Assembly.GetCallingAssembly());
                startup.ConfigureServices(services, applicationSupplier, settingsFile);
                services.AddSingleton<ITimeoutConfiguration>(new CustomTimeoutConfiguration(settingsFile));
                return services;
            }

            private static Func<IServiceProvider, IApplication> StartApplicationFunction => (services) => throw new NotImplementedException();
        }

        private class CustomTimeoutConfiguration : TimeoutConfiguration
        {
            public CustomTimeoutConfiguration(JsonFile settingsFile) : base(settingsFile)
            {
                CustomTimeout = SpecialTimeoutValue;
            }

            public TimeSpan CustomTimeout { get; }
        }
    }
}
