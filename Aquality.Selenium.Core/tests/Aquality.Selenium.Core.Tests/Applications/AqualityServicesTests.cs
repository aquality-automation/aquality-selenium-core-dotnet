using System;
using System.Reflection;
using System.Threading;
using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Aquality.Selenium.Core.Tests.Applications
{
    public class AqualityServicesTests
    {
        private const string SpecialSettingsFile = "special";
        private const string SpecialLanguageValue = "special";
        private static readonly TimeSpan SpecialTimeoutValue = TimeSpan.FromDays(1);
        private const string SpecialLogger = "SpecialLogger";

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices()
        {
            Assert.That(TestAqualityServices.ServiceProvider.GetService<ITimeoutConfiguration>(), Is.InstanceOf<TestTimeoutConfiguration>());
        }

        [Test]
        public void Should_BePossibleTo_GetCustomValues()
        {
            var timeoutConfiguration = TestAqualityServices.ServiceProvider.GetService<ITimeoutConfiguration>() as TestTimeoutConfiguration;
            Assert.That(timeoutConfiguration.CustomTimeout, Is.EqualTo(SpecialTimeoutValue));
        }

        [Test]
        public void Should_BePossibleTo_GetCustomLoggerValues()
        {
            TestAqualityServices.SetStartup(new CustomStartup());
            var timeoutConfiguration = TestAqualityServices.ServiceProvider.GetService<ILoggerConfiguration>() as CustomLoggerConfiguration;
            Assert.That(timeoutConfiguration.CustomLogger, Is.EqualTo(SpecialLogger));
        }

        [Test]
        public void Should_BePossibleTo_RegisterCustomServices_WithCustomSettingsFile()
        {
            Assert.That(TestAqualityServices.ServiceProvider.GetService<ILoggerConfiguration>().Language, Is.EqualTo(SpecialLanguageValue));
        }

        private class TestAqualityServices : AqualityServices<IApplication>
        {
            private static readonly ThreadLocal<TestStartup> startup = new();

            private static IApplication Application => GetApplication(StartApplicationFunction, () => startup.Value.ConfigureServices(new ServiceCollection(), services => Application));

            public static IServiceProvider ServiceProvider => GetServiceProvider(services => Application,
                () => startup.Value.ConfigureServices(new ServiceCollection(), services => Application));

            public static void SetStartup(Startup startup)
            {
                if (startup != null)
                {
                    TestAqualityServices.startup.Value = (TestStartup)startup;
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

        private class TestTimeoutConfiguration(ISettingsFile settingsFile) : TimeoutConfiguration(settingsFile)
        {
            public TimeSpan CustomTimeout { get; } = SpecialTimeoutValue;
        }

        private class CustomLoggerConfiguration(ISettingsFile settingsFile) : LoggerConfiguration(settingsFile)
        {
            public string CustomLogger { get; } = SpecialLogger;
        }
    }
}
