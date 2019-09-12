using Aquality.Selenium.Core.Utilities;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    [NonParallelizableAttribute]
    public class JsonFileTests
    {
        [Test]
        public void GetValue_ShouldBe_PossibleTo_OverrideValueFromEnvVar()
        {
            Environment.SetEnvironmentVariable("timeouts.timeoutCondition", "500");
            var timeoutConfiguration = GetJsonFile();
            Assert.AreEqual(500, timeoutConfiguration.GetValue<int>(".timeouts.timeoutCondition"));
        }

        [Test]
        public void GetValue_ShouldThrow_ArgumentException_InCaseOfEnvVarIncorrectFormat()
        {
            Environment.SetEnvironmentVariable("timeouts.timeoutPollingInterval", "incorrect_env_var");
            Assert.Throws<ArgumentException>(() => GetJsonFile().GetValue<int>(".timeouts.timeoutPollingInterval"));
        }

        [Test]
        public void GetValue_ShouldThrow_ArgumentException_InCaseOfNotValidKey()
        {
            Assert.Throws<ArgumentException>(() => GetJsonFile().GetValue<int>(".timeouts.invalidKey"));
        }

        [Test]
        public void GetValueList_ShouldThrow_ArgumentException_InCaseOfNotValidKey()
        {
            Assert.Throws<ArgumentException>(() => GetJsonFile().GetValueList<string>(".driverSettings.args"));
        }

        private JsonFile GetJsonFile()
        {
            return new JsonFile("settings.custom.json");
        }
    }
}
