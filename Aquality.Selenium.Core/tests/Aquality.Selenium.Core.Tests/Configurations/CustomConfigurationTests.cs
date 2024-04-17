using Aquality.Selenium.Core.Configurations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Aquality.Selenium.Core.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace Aquality.Selenium.Core.Tests.Configurations
{
    [Parallelizable(ParallelScope.All)]
    public class CustomConfigurationTests
    {
        private ServiceProvider ServiceProvider { get; set; }
        private static FakeSettingFile SettingFile => new();

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            new Startup().ConfigureServices(services, applicationProvider:
                serviceCollection => throw new InvalidOperationException($"Application should not be required for {TestContext.CurrentContext.Test.FullName}"),
                settings: SettingFile);
            ServiceProvider = services.BuildServiceProvider();
        }

        [Test]
        public void Should_BePossible_ToGetRetryConfig_WithXmlConfigFile()
        {
            var retryConfig = ServiceProvider.GetService<IRetryConfiguration>();
            var timeoutConfig = ServiceProvider.GetService<ITimeoutConfiguration>();
            var loggerConfig = ServiceProvider.GetService<ILoggerConfiguration>();
            Assert.Multiple(() =>
            {
                Assert.That(retryConfig.PollingInterval, Is.EqualTo(default(TimeSpan)), "Retry config should be received from custom setting file");
                Assert.That(timeoutConfig.PollingInterval, Is.EqualTo(default(TimeSpan)), "Timeout config should be received from custom setting file");
                Assert.That(loggerConfig.Language, Is.EqualTo("en"), "Logger config should be received from custom setting file");
            });
        }

        private class FakeSettingFile : ISettingsFile
        {
            public T GetValue<T>(string path)
            {
                return default;
            }

            public IReadOnlyList<T> GetValueList<T>(string path)
            {
                return new List<T>();
            }

            public IReadOnlyDictionary<string, T> GetValueDictionary<T>(string path)
            {
                return new Dictionary<string, T>();
            }

            public bool IsValuePresent(string path)
            {
                return false;
            }
        }
    }
}
