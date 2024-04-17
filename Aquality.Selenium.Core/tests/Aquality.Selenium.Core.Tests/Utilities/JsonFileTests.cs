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
        private static JsonSettingsFile CustomSettings => new("settings.custom.json");
        private static JsonSettingsFile AddedParamsSettings => new("settings.addedparams.json");

        [Test]
        [NonParallelizable]
        public void GetValue_ShouldBe_PossibleTo_OverrideValueFromEnvVar()
        {
            Environment.SetEnvironmentVariable("timeouts.timeoutCondition", "500");
            Assert.That(CustomSettings.GetValue<int>(".timeouts.timeoutCondition"), Is.EqualTo(500));
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
            Assert.That(AddedParamsSettings.GetValue<string>(".driverSettings.chrome.webDriverVersion"), Is.EqualTo("Latest"),
                "Value was received successively");
        }

        [Test]
        public void Should_BePossibleTo_GetValueList()
        {
            var expectedValues = new List<string> {"1", "2", "3"};
            Assert.That(AddedParamsSettings.GetValueList<string>(".driverSettings.chrome.startArguments"), Is.EqualTo(expectedValues),
                "List of values was received successively");
        }

        [Test]
        [NonParallelizable]
        public void Should_BePossibleTo_OverrideListOfValues_FromEnvVar()
        {
            const string jsonPath = ".driverSettings.chrome.startArguments";
            Environment.SetEnvironmentVariable(jsonPath, "1, 3, 5");
            var expectedValues = new List<string> { "1", "3", "5" };
            Assert.That(AddedParamsSettings.GetValueList<string>($".{jsonPath}"), Is.EqualTo(expectedValues), 
                "List of values was overridden successively");
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
            Assert.That(AddedParamsSettings.GetValueDictionary<object>(".driverSettings.chrome.options"),
                Is.EqualTo(expectedDict),
                "Dictionary of keys and values was received successively");
        }

        [Test]
        public void Should_BePossibleTo_GetNotEmptyValueDictionary()
        {
            var expectedDict = new Dictionary<string, object>
            {
                {"intl.accept_languages", "en"},
                {"profile.default_content_settings.popups", "0"},
                {"disable-popup-blocking", "true"}
            };
            Assert.That(AddedParamsSettings.GetValueDictionaryOrEmpty<object>(".driverSettings.chrome.options"),
                Is.EqualTo(expectedDict),
                "Dictionary of keys and values was received successively");
        }

        [Test]
        public void Should_BePossibleTo_GetEmptyValueDictionary()
        {
            var expectedDict = new Dictionary<string, object>();
            Assert.That(AddedParamsSettings.GetValueDictionaryOrEmpty<object>(".some.absent.path"), 
                Is.EqualTo(expectedDict), 
                "Dictionary of keys and values was not empty");
        }

        [Test]
        [NonParallelizable]
        public void Should_BePossibleTo_OverrideDictionaryOfValues_FromEnvVar()
        {
            CheckOverrideDictionaryFromEnvVar<string>();
        }

        [Test]
        [NonParallelizable]
        public void Should_BePossibleTo_OverrideDictionaryOfObjects_FromEnvVar()
        {
            CheckOverrideDictionaryFromEnvVar<object>();
        }

        private static void CheckOverrideDictionaryFromEnvVar<T>()
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

            Assert.That(AddedParamsSettings.GetValueDictionary<T>(".driverSettings.chrome.options"),
                Is.EqualTo(expectedDict),
                "Dictionary of keys and values was overriden successively");
        }

        [TestCase(".timeouts.timeoutImplicit", true)]
        [TestCase(".timeouts.invalidKey", false)]
        public void Should_BePossibleTo_CheckIsValueExists_InSettings(string key, bool shouldExist)
        {
            Assert.That(CustomSettings.IsValuePresent(key),
                Is.EqualTo(shouldExist),
                $"{key} should have exist status '{shouldExist}' in settings");
        }
    }
}
