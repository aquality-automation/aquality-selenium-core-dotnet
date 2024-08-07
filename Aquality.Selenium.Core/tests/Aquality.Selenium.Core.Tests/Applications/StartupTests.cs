﻿using Aquality.Selenium.Core.Applications;
using Aquality.Selenium.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Applications
{
    [Parallelizable(ParallelScope.None)]
    public class StartupTests
    {
        private const string ProfileVariableName = "profile";
        private const string ProfileName = "embeddedresource";

        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, ProfileName);
        }

        [TearDown]
        public void CleanUp()
        {
            Environment.SetEnvironmentVariable(ProfileVariableName, null);
        }

        [Test]
        public void Should_GetConfiguration_FromCustomConfigurationProfile()
        {
            var services = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(services, applicationProvider:
                serviceCollection => throw new InvalidOperationException("Application should not be required"),
                settings: startup.GetSettings());
            Assert.That(services.BuildServiceProvider().GetService<ILoggerConfiguration>().Language, Is.EqualTo("embedded"));
        }
    }
}
