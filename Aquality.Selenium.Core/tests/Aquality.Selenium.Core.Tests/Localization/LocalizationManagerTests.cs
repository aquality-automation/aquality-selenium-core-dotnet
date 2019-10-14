using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Localization;
using Aquality.Selenium.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Aquality.Selenium.Core.Tests.Localization
{
    public sealed class LocalizationManagerTests : TestWithoutApplication
    {
        private static readonly string[] SupportedLanguages = { "be", "en", "ru" };
        private static readonly string[] KeysWithoutParams = 
        { 
            "loc.clicking", 
            "loc.get.text", 
        };
        private static readonly string[] KeysWithParams =
        {
            "loc.el.getattr",
            "loc.text.sending.keys",
            "loc.no.elements.found.in.state",
            "loc.no.elements.found.by.locator",
            "loc.elements.were.found.but.not.in.state",
            "loc.elements.found.but.should.not"
        };

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager_ForClicking()
        {
            Assert.AreEqual("Clicking", ServiceProvider.GetService<ILocalizationManager>().GetLocalizedMessage("loc.clicking"));
        }

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager_ForUnknownKey()
        {
            var unknownKey = "loc.unknown.fake.key";
            Assert.AreEqual(unknownKey, ServiceProvider.GetService<ILocalizationManager>().GetLocalizedMessage(unknownKey));
        }

        [Test]
        public void Should_ReturnNonKeyValues_AndNotEmptyValues_ForKeysWithoutParams([ValueSource(nameof(SupportedLanguages))] string language, [ValueSource(nameof(KeysWithoutParams))] string key)
        {
            var configuration = new DynamicConfiguration
            {
                Language = language
            };
            var localizedValue = new LocalizationManager(configuration, Logger.Instance).GetLocalizedMessage(key);
            Assert.AreNotEqual(key, localizedValue, "Value should be defined in resource files");
            Assert.IsNotEmpty(localizedValue, "Value should not be empty");
        }

        [Test]
        public void Should_ReturnNonKeyValues_AndNotEmptyValues_ForKeysWithParams([ValueSource(nameof(SupportedLanguages))] string language, [ValueSource(nameof(KeysWithParams))] string key)
        {
            var configuration = new DynamicConfiguration
            {
                Language = language
            };
            var paramsArray = new[] { "a", "b", "c"};
            var localizedValue = new LocalizationManager(configuration, Logger.Instance).GetLocalizedMessage(key, paramsArray);
            Assert.AreNotEqual(key, localizedValue, "Value should be defined in resource files");
            Assert.IsNotEmpty(localizedValue, "Value should not be empty");
            Assert.IsTrue(localizedValue.Contains(paramsArray[0]), "Value should contain at least first parameter");
        }

        [Test]
        public void Should_ThrowsFormatException_WhenKeysRequireParams([ValueSource(nameof(SupportedLanguages))] string language, [ValueSource(nameof(KeysWithParams))] string key)
        {
            var configuration = new DynamicConfiguration
            {
                Language = language
            };
            Assert.Throws<FormatException>(() => 
                new LocalizationManager(configuration, Logger.Instance).GetLocalizedMessage(key));
        }

        private class DynamicConfiguration : ILoggerConfiguration
        {
            public string Language { get; set; }
        }
    }
}
