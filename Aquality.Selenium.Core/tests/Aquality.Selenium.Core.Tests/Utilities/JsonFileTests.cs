using Aquality.Selenium.Core.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Utilities
{
    [Parallelizable]
    public class JsonFileTests
    {
        private static JsonSettingsFile CustomSettings => new("settings.custom.json");
        private static JsonSettingsFile AddedParamsSettings => new("settings.addedparams.json");
        private static JsonSettingsFile ExtendedSettings => new("settings.extended.json");

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
                Is.EquivalentTo(expectedDict),
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
                Is.EquivalentTo(expectedDict),
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
        
        [Test]
        public void ExtendedSettings_Should_GetDeepNestedValue()
        {
            Assert.That(ExtendedSettings.GetValue<string>(".level1.level2.level3.level4.deepValue"), Is.EqualTo("found"),
                "Deep nested value should be accessible");
        }

        [Test]
        public void ExtendedSettings_Should_GetArrayValue()
        {
            var expectedArray = new List<string> { "a", "b", "c" };
            Assert.That(ExtendedSettings.GetValueList<string>(".arrays.simpleArray"), Is.EqualTo(expectedArray),
                "Simple array should be accessible");
        }

        [Test]
        public void ExtendedSettings_Should_GetValueWithSpecialChars()
        {
            Assert.That(ExtendedSettings.GetValue<string>(".specialChars.key-with-dashes"), Is.EqualTo("dash-value"),
                "Key with dashes should be accessible");
            Assert.That(ExtendedSettings.GetValue<string>(".specialChars.[key.with.dots]"), Is.EqualTo("dot-value"),
                "Key with dots should be accessible");
            Assert.That(ExtendedSettings.GetValue<string>(".specialChars.key with spaces"), Is.EqualTo("space-value"),
                "Key with spaces should be accessible");
            Assert.That(ExtendedSettings.GetValue<string>(".specialChars.key'with'quotes"), Is.EqualTo("quote-value"),
                "Key with quotes should be accessible");
        }

        [Test]
        public void ExtendedSettings_Should_GetDifferentDataTypes()
        {
            Assert.That(ExtendedSettings.GetValue<string>(".dataTypes.stringValue"), Is.EqualTo("text"),
                "String value should be accessible");
            Assert.That(ExtendedSettings.GetValue<int>(".dataTypes.intValue"), Is.EqualTo(42),
                "Integer value should be accessible");
            Assert.That(ExtendedSettings.GetValue<double>(".dataTypes.doubleValue"), Is.EqualTo(3.14),
                "Double value should be accessible");
            Assert.That(ExtendedSettings.GetValue<bool>(".dataTypes.boolValue"), Is.EqualTo(true),
                "Boolean value should be accessible");
        }

        [Test]
        public void ExtendedSettings_Should_GetMixedArrayValues()
        {
            var expectedMixedArray = new object[] { 1, "string", true, null };
            var actualArray = ExtendedSettings.GetValueList<object>(".arrays.mixedArray");
    
            Assert.That(actualArray, Has.Count.EqualTo(4), "Array should have 4 elements");
            Assert.That(actualArray[0], Is.EqualTo(1), "First element should be 1");
            Assert.That(actualArray[1], Is.EqualTo("string"), "Second element should be 'string'");
            Assert.That(actualArray[2], Is.EqualTo(true), "Third element should be true");
            Assert.That(actualArray[3], Is.Null, "Fourth element should be null");
        }

        [Test]
        public void ExtendedSettings_Should_CheckValuePresence()
        {
            Assert.That(ExtendedSettings.IsValuePresent(".level1.level2.level3.level4.deepValue"), Is.True,
                "Deep nested value should be present");
            Assert.That(ExtendedSettings.IsValuePresent(".nonExistentKey"), Is.False,
                "Non-existent key should not be present");
            Assert.That(ExtendedSettings.IsValuePresent(".dataTypes.nullValue"), Is.True,
                "Null value should be present");
        }

        [Test]
        public void ExtendedSettings_Should_GetSpecialCharsDictionary()
        {
            var expectedDict = new Dictionary<string, object>
            {
                {"key-with-dashes", "dash-value"},
                {"key.with.dots", "dot-value"},
                {"key with spaces", "space-value"},
                {"key'with'quotes", "quote-value"}
            };
            Assert.That(ExtendedSettings.GetValueDictionary<string>(".specialChars"),
                Is.EquivalentTo(expectedDict),
                "Dictionary with special character keys should be accessible");
        }

        [Test]
        public void ExtendedSettings_Should_GetDataTypesDictionary()
        {
            var expectedDict = new Dictionary<string, object>
            {
                {"stringValue", "text"},
                {"intValue", 42},
                {"doubleValue", 3.14},
                {"boolValue", true},
                {"nullValue", null}
            };
            Assert.That(ExtendedSettings.GetValueDictionary<object>(".dataTypes"),
                Is.EquivalentTo(expectedDict),
                "Dictionary with different data types should be accessible");
        }
        
        [Test]
        public void GetValue_WithValidEnumStringValue_ReturnsCorrectEnum()
        {
            var result = ExtendedSettings.GetValue<TestEnum>(".enumValues.stringValue");
            Assert.That(result, Is.EqualTo(TestEnum.Value1), "Enum value should be correct");;
        }
        
        private enum TestEnum
        {
            Value1
        }
    }
}
