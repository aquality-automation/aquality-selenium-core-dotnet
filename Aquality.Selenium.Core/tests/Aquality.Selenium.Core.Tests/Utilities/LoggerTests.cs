using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using NLog.Targets;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.IO;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    [NonParallelizable]
    public class LoggerTests : TestWithBrowser
    {
        private const string AddTargetLogFile = "AddTargetTestLog.log";
        private const string RemoveTargetLogFile = "RemoveTargetTestLog.log";
        private const string TestMessage = "test message";
        private const string LogPageSourceEnvironmentVariable = "logger.logPageSource";

        [SetUp]
        public void Setup()
        {
            File.Delete(AddTargetLogFile);
            File.Delete(RemoveTargetLogFile);
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(LogPageSourceEnvironmentVariable, null);
        }

        [Test]
        public void Should_ThrowException_And_LogPageSource_WhenIsEnabledAndElementAbsent()
        {
            File.Delete(AddTargetLogFile);
            Environment.SetEnvironmentVariable(LogPageSourceEnvironmentVariable, true.ToString());
            Logger.Instance.AddTarget(GetTarget(AddTargetLogFile));
            var element = new Label(By.Name("Absent element"), "Absent element", Elements.ElementState.ExistsInAnyState);            
            Assert.Throws(Is.AssignableFrom(typeof(NoSuchElementException)).And.Message.Contains(element.Name),
                () => element.GetElement(TimeSpan.Zero), 
                "Attempt to get absent element should throw an exception");
            Assert.True(File.Exists(AddTargetLogFile),
                $"Target wasn't added. File '{AddTargetLogFile}' doesn't exist.");
            var log = File.ReadAllText(AddTargetLogFile).Trim();
            StringAssert.Contains("Page source:", log, "Log file should contain logged page source");
        }

        [Test]
        public void Should_NotLogPageSource_AndThrowException_WhenIsDisabledAndElementAbsent()
        {
            File.Delete(AddTargetLogFile);
            Environment.SetEnvironmentVariable(LogPageSourceEnvironmentVariable, false.ToString());
            Logger.Instance.AddTarget(GetTarget(AddTargetLogFile));
            var element = new Label(By.Name("Absent element"), "Absent element", Elements.ElementState.ExistsInAnyState);
            Assert.Throws<NoSuchElementException>(() => element.GetElement(TimeSpan.Zero), "Attempt to get absent element should throw an exception");
            Assert.True(File.Exists(AddTargetLogFile),
                $"Target wasn't added. File '{AddTargetLogFile}' doesn't exist.");
            var log = File.ReadAllText(AddTargetLogFile).Trim();
            StringAssert.DoesNotContain("Page source:", log, "Log file should not contain logged page source");
        }

        [Test]
        public void Should_BePossibleTo_AddTarget()
        {
            Logger.Instance.AddTarget(GetTarget(AddTargetLogFile)).Info(TestMessage);
            Assert.True(File.Exists(AddTargetLogFile),
                $"Target wasn't added. File '{AddTargetLogFile}' doesn't exist.");
            var log = File.ReadAllText(AddTargetLogFile).Trim();
            Assert.True(log.Equals(TestMessage),
                $"Target wasn't added. File doesn't contain message: '{TestMessage}'.");
        }

        [Test]
        public void Should_BePossibleTo_RemoveTarget()
        {
            var target = GetTarget(RemoveTargetLogFile);
            Logger.Instance.AddTarget(target).RemoveTarget(target).Info(TestMessage);
            Assert.False(File.Exists(RemoveTargetLogFile),
                $"Target wasn't removed. File '{RemoveTargetLogFile}' exists.");
        }

        private static Target GetTarget(string filePath)
        {
            return new FileTarget
            {
                Name = Guid.NewGuid().ToString(),
                FileName = filePath,
                Layout = "${message}"
            };
        }
    }
}
