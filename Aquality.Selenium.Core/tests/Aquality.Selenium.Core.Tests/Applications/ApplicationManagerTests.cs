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
        private const string SpecialLogger = "SpecialLogger";

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices()
        {
            Assert.IsInstanceOf<TestTimeoutConfiguration>(TestApplicationManager.ServiceProvider.GetService<ITimeoutConfiguration>());
        }

        [Test]
        public void Should_BePossibleTo_GetCustomValues()
        {
            var timeoutConfiguration = TestApplicationManager.ServiceProvider.GetService<ITimeoutConfiguration>() as TestTimeoutConfiguration;
            Assert.AreEqual(SpecialTimeoutValue,  timeoutConfiguration.CustomTimeout);
        }

        [Test]
        public void Should_BePossibleTo_GetCustomLoggerValues()
        {
            TestApplicationManager.SetStartup(new CustomStartup());
            var timeoutConfiguration = TestApplicationManager.ServiceProvider.GetService<ILoggerConfiguration>() as CustomLoggerConfiguration;
            Assert.AreEqual(SpecialLogger, timeoutConfiguration.CustomLogger);
        }

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices_WithCustomSettingsFile()
        {
            Assert.AreEqual(SpecialLanguageValue, TestApplicationManager.ServiceProvider.GetService<ILoggerConfiguration>().Language);
        }

        private class TestApplicationManager : ApplicationManager<IApplication>
        {
            private static Startup startup = new TestStartup();

            private static IApplication Application => GetApplication(StartApplicationFunction, () => startup.ConfigureServices(new ServiceCollection(), services => Application));

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application,
                () => startup.ConfigureServices(new ServiceCollection(), services => Application));

            public static void SetStartup(Startup startup)
            {
                if (startup != null)
                {
                    TestApplicationManager.startup = startup;
                }
            }

            private static Func<IServiceProvider, IApplication> StartApplicationFunction => (services) => throw new NotImplementedException();
        }

        private class TestStartup : Startup
        {
            public override IServiceCollection ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider, ISettingsFile settings = null)
            {
                var settingsFile = new JsonSettingsFile($"Resources.settings.{SpecialSettingsFile}.json", Assembly.GetExecutingAssembly());
                base.ConfigureServices(services, applicationProvider, settingsFile);
                services.AddSingleton<ITimeoutConfiguration>(new TestTimeoutConfiguration(settingsFile));
                return services;
            }
        }

        private class CustomStartup : TestStartup
        {
            public override IServiceCollection ConfigureServices(IServiceCollection services, Func<IServiceProvider, IApplication> applicationProvider, ISettingsFile settings = null)
            {
                var settingsFile = new JsonSettingsFile($"Resources.settings.{SpecialSettingsFile}.json", Assembly.GetExecutingAssembly());
                base.ConfigureServices(services, applicationProvider, settingsFile);
                services.AddSingleton<ILoggerConfiguration>(new CustomLoggerConfiguration(settingsFile));
                return services;
            }
        }

        private class TestTimeoutConfiguration : TimeoutConfiguration
        {
            public TestTimeoutConfiguration(ISettingsFile settingsFile) : base(settingsFile)
            {
                CustomTimeout = SpecialTimeoutValue;
            }

            public TimeSpan CustomTimeout { get; }
        }

        private class CustomLoggerConfiguration : LoggerConfiguration
        {
            public CustomLoggerConfiguration(ISettingsFile settingsFile) : base(settingsFile)
            {
                CustomLogger = SpecialLogger;
            }

            public string CustomLogger { get; }
        }
    }
}
