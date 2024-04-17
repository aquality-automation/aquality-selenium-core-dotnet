using Aquality.Selenium.Core.Configurations;
using NUnit.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Aquality.Selenium.Core.Tests.Configurations
{
    [Parallelizable(ParallelScope.None)]
    public class EnvConfigurationTests : TestWithoutApplication
    {
        private const string ProfileVariableName = "profile";
        private const string ProfileName = "custom";

        [SetUp]
        public new void SetUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, ProfileName);
        }

        [TearDown]
        public void CleanUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, null);
            Environment.SetEnvironmentVariable("timeouts.timeoutImplicit", null);
            Environment.SetEnvironmentVariable("timeouts.timeoutCondition", null);
            Environment.SetEnvironmentVariable("timeouts.timeoutPollingInterval", null);
            Environment.SetEnvironmentVariable("timeouts.timeoutCommand", null);
            Environment.SetEnvironmentVariable("retry.number", null);
            Environment.SetEnvironmentVariable("retry.pollingInterval", null);
            Environment.SetEnvironmentVariable("logger.language", null);
            Environment.SetEnvironmentVariable("elementCache.isEnabled", null);
        }

        [Test]
        public void Should_BePossible_ToOverrideTimeouts_WithEnvVariables()
        {
            const string testValue = "1000";
            var expectedValueSec = TimeSpan.FromSeconds(1000);
            var expectedValueMillis = TimeSpan.FromMilliseconds(1000);
            const string messageTmp = "{0} timeout should be overridden with env variable";
            Environment.SetEnvironmentVariable("timeouts.timeoutImplicit", testValue);
            Environment.SetEnvironmentVariable("timeouts.timeoutCondition", testValue);
            Environment.SetEnvironmentVariable("timeouts.timeoutPollingInterval", testValue);
            Environment.SetEnvironmentVariable("timeouts.timeoutCommand", testValue);
            base.SetUp();

            var config = ServiceProvider.GetService<ITimeoutConfiguration>();
            Assert.Multiple(() =>
            {
                Assert.That(config.Command, Is.EqualTo(expectedValueSec), string.Format(messageTmp, "Command"));
                Assert.That(config.Condition, Is.EqualTo(expectedValueSec), string.Format(messageTmp, "Condition"));
                Assert.That(config.Implicit, Is.EqualTo(expectedValueSec), string.Format(messageTmp, "Implicit"));
                Assert.That(config.PollingInterval, Is.EqualTo(expectedValueMillis), string.Format(messageTmp, "PollingInterval"));
            });
        }

        [Test]
        public void Should_BePossible_ToOverrideRetryConfig_WithEnvVariables()
        {
            const string testValue = "1000";
            var expectedInterval = TimeSpan.FromMilliseconds(1000);
            const int expectedNumber = 1000;
            const string messageTmp = "Retry value '{0}' should be overridden with env variable";
            Environment.SetEnvironmentVariable("retry.number", testValue);
            Environment.SetEnvironmentVariable("retry.pollingInterval", testValue);
            base.SetUp();

            var config = ServiceProvider.GetService<IRetryConfiguration>();
            Assert.Multiple(() =>
            {
                Assert.That(config.PollingInterval, Is.EqualTo(expectedInterval), string.Format(messageTmp, "PollingInterval"));
                Assert.That(config.Number, Is.EqualTo(expectedNumber), string.Format(messageTmp, "Number"));
            });
        }

        [Test]
        public void Should_BePossible_ToOverrideLoggerConfig_WithEnvVariables()
        {
            const string testValue = "testLang";
            Environment.SetEnvironmentVariable("logger.language", testValue);
            base.SetUp();

            var config = ServiceProvider.GetService<ILoggerConfiguration>();
            Assert.That(config.Language, Is.EqualTo(testValue), "Logger language value should be overridden with env variable");
        }

        [Test]
        public void Should_ReturnFalse_InElementCacheEnable_WhenElementCacheIsAbsent()
        {
            Assert.That(ServiceProvider.GetRequiredService<IElementCacheConfiguration>().IsEnabled, Is.False,
                nameof(Should_ReturnFalse_InElementCacheEnable_WhenElementCacheIsAbsent));
        }

        [Test]
        public void Should_ReturnTrue_InElementCacheEnable_WhenElementCacheIsDisabled()
        {
            Environment.SetEnvironmentVariable("elementCache.isEnabled", "true");
            Assert.That(ServiceProvider.GetRequiredService<IElementCacheConfiguration>().IsEnabled,
                nameof(Should_ReturnTrue_InElementCacheEnable_WhenElementCacheIsDisabled));
        }

        [Test]
        public void Should_ReturnFalse_InElementCacheEnable_WhenElementCacheIsDisabled()
        {
            Environment.SetEnvironmentVariable("elementCache.isEnabled", "false");
            Assert.That(ServiceProvider.GetRequiredService<IElementCacheConfiguration>().IsEnabled, Is.False,
                nameof(Should_ReturnFalse_InElementCacheEnable_WhenElementCacheIsDisabled));
        }
    }
}
