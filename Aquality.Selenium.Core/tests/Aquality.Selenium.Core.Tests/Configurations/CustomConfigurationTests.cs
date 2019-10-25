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
        private static FakeSettingFile SettingFile => new FakeSettingFile();

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            Startup.ConfigureServices(services, applicationSupplier:
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
                Assert.AreEqual(default(TimeSpan), retryConfig.PollingInterval, "Retry config should be received from custom setting file");
                Assert.AreEqual(default(TimeSpan), timeoutConfig.PollingInterval, "Timeout config should be received from custom setting file");
                Assert.AreEqual("en", loggerConfig.Language, "Logger config should be received from custom setting file");
            });
        }

        private class FakeSettingFile : ISettingsFile
        {
            public T GetValue<T>(string path)
            {
                return default(T);
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
