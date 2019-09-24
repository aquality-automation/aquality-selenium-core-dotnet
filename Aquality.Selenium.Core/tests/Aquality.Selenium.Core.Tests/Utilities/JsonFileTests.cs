using Aquality.Selenium.Core.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Aquality.Selenium.Core.Configurations;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    [Parallelizable]
    public class JsonFileTests
    {
        private static ISettingsFile CustomSettings => new JsonSettingsFile("settings.custom.json");
        private static ISettingsFile AddedParamsSettings => new JsonSettingsFile("settings.addedparams.json");

        [Test]
        [NonParallelizable]
        public void GetValue_ShouldBe_PossibleTo_OverrideValueFromEnvVar()
        {
            Environment.SetEnvironmentVariable("timeouts.timeoutCondition", "500");
            Assert.AreEqual(500, CustomSettings.GetValue<int>(".timeouts.timeoutCondition"));
        }

        [Test]
        [NonParallelizable]
        public void GetValue_ShouldThrow_ArgumentException_InCaseOfEnvVarIncorrectFormat()
        {
            Environment.SetEnvironmentVariable("timeouts.timeoutPollingInterval", "incorrect_env_var");
            Assert.Throws<ArgumentException>(() => CustomSettings.GetValue<int>(".timeouts.timeoutPollingInterval"));
        }

        [Test]
        public void GetValue_ShouldThrow_ArgumentException_InCaseOfNotValidKey()
        {
            Assert.Throws<ArgumentException>(() => CustomSettings.GetValue<int>(".timeouts.invalidKey"));
        }

        [Test]
        public void GetValueList_ShouldThrow_ArgumentException_InCaseOfNotValidKey()
        {
            Assert.Throws<ArgumentException>(() => CustomSettings.GetValueList<string>(".driverSettings.args"));
        }

        [Test]
        public void Should_BePossibleTo_GetValue()
        {
            Assert.AreEqual("Latest", AddedParamsSettings.GetValue<string>(".driverSettings.chrome.webDriverVersion"),
                "Value was received successively");
        }

        [Test]
        public void Should_BePossibleTo_GetValueList()
        {
            var expectedValues = new List<string> {"1", "2", "3"};
            Assert.AreEqual(expectedValues,
                AddedParamsSettings.GetValueList<string>(".driverSettings.chrome.startArguments"),
                "List of values was received successively");
        }

        [Test]
        [NonParallelizable]
        public void Should_BePossibleTo_OverrideListOfValues_FromEnvVar()
        {
            const string jsonPath = ".driverSettings.chrome.startArguments";
            Environment.SetEnvironmentVariable(jsonPath, "1, 3, 5");
            var expectedValues = new List<string> { "1", "3", "5" };
            Assert.AreEqual(expectedValues,
                AddedParamsSettings.GetValueList<string>($".{jsonPath}"),
                "List of values was overriden successively");
        }

        [Test]
        public void Should_BePossibleTo_GetValueDictionary()
        {
            var expectedDict = new Dictionary<string, object>
            {
                {"intl.accept_languages", "en"},
                {"profile.default_content_settings.popups", "0"},
                {"disable-popup-blocking", "true"}
            };
            Assert.AreEqual(expectedDict,
                AddedParamsSettings.GetValueDictionary<object>(".driverSettings.chrome.options"),
                "Dictionary of keys and values was received successively");
        }

        [Test]
        [NonParallelizable]
        public void Should_BePossibleTo_OverrideDictionaryOfValues_FromEnvVar()
        {
            var expectedDict = new Dictionary<string, object>
            {
                {"intl.accept_languages", "1"},
                {"profile.default_content_settings.popups", "true"},
                {"disable-popup-blocking", "bla"}
            };
            Environment.SetEnvironmentVariable("driverSettings.chrome.options.intl.accept_languages", "1");
            Environment.SetEnvironmentVariable("driverSettings.chrome.options.profile.default_content_settings.popups", "true");
            Environment.SetEnvironmentVariable("driverSettings.chrome.options.disable-popup-blocking", "bla");

            Assert.AreEqual(expectedDict,
                AddedParamsSettings.GetValueDictionary<string>(".driverSettings.chrome.options"),
                "Dictionary of keys and values was overriden successively");
        }

        [TestCase(".timeouts.timeoutImplicit", true)]
        [TestCase(".timeouts.invalidKey", false)]
        public void Should_BePossibleTo_CheckIsValueExists_InSettings(string key, bool shouldExist)
        {
            Assert.AreEqual(shouldExist, CustomSettings.IsValuePresent(key), $"{key} should have exist status '{shouldExist}' in settings");
        }
    }
}
