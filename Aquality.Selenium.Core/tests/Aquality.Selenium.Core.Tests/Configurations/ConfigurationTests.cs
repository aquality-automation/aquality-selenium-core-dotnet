using Aquality.Selenium.Core.Configurations;
using NUnit.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Aquality.Selenium.Core.Tests.Configurations
{
    [Parallelizable(ParallelScope.None)]
    public class ConfigurationTests : TestWithoutApplication
    {
        private const string LoggerLang = "be";
        private const string ProfileVariableName = "profile";
        private const string ProfileName = "custom";

        [SetUp]
        public new void SetUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, ProfileName);
            base.SetUp();
        }

        [TearDown]
        public void CleanUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, null);
        }

        [Test]
        public void Should_GetConfiguration_FromCustomConfigurationProfile()
        {
            Assert.AreEqual(LoggerLang, ServiceProvider.GetService<ILoggerConfiguration>().Language);
        }
    }
}
