using Aquality.Selenium.Core.Applications;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public abstract class TestWithoutApplication
    {
        protected ServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            new Startup().ConfigureServices(services, applicationProvider:
                serviceCollection => throw new InvalidOperationException($"Application should not be required for {TestContext.CurrentContext.Test.FullName}"));
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
