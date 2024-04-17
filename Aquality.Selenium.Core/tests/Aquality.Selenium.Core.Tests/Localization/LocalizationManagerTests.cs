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
        private const string ClickingKey = "loc.clicking";
        private const string ClickingValueBe = "Націскаем";
        private const string ClickingValueEn = "Clicking";
        private static readonly string[] SupportedLanguages = ["be", "en", "ru"];
        private static readonly string[] KeysWithoutParams = 
        [ 
            ClickingKey, 
            "loc.get.text",
            "loc.el.state.displayed",
            "loc.el.state.not.displayed",
            "loc.el.state.exist",
            "loc.el.state.not.exist",
            "loc.el.state.enabled",
            "loc.el.state.not.enabled",
            "loc.el.state.clickable",
            "loc.el.visual.getimage",
            "loc.el.visual.getlocation",
            "loc.el.visual.getsize"
        ];
        private static readonly string[] KeysWithParams =
        [
            "loc.el.getattr",
            "loc.el.attr.value",
            "loc.text.value",
            "loc.text.sending.keys",
            "loc.no.elements.found.in.state",
            "loc.no.elements.with.name.found.by.locator",
            "loc.elements.were.found.but.not.in.state",
            "loc.elements.with.name.found.but.should.not",
            "loc.search.of.elements.failed",
            "loc.wait.for.state",
            "loc.wait.for.state.failed",
            "loc.el.visual.image.value",
            "loc.el.visual.location.value",
            "loc.el.visual.size.value",
            "loc.el.visual.getdifference",
            "loc.el.visual.getdifference.withthreshold",
            "loc.el.visual.difference.value",
            "loc.form.dump.save",
            "loc.form.dump.imagenotsaved",
            "loc.form.dump.compare",
            "loc.form.dump.elementnotfound",
            "loc.form.dump.elementsmissedindump",
            "loc.form.dump.elementsmissedonform",
            "loc.form.dump.unprocessedelements",
            "loc.form.dump.compare.result"
        ];

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager_ForClicking_CustomConfig()
        {
            Environment.SetEnvironmentVariable("profile", "custom");
            SetUp();
            Environment.SetEnvironmentVariable("profile", string.Empty);
            Assert.That(ServiceProvider.GetService<ILocalizationManager>().GetLocalizedMessage(ClickingKey), Is.EqualTo(ClickingValueBe));
        }

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager_ForClicking()
        {
            Assert.That(ServiceProvider.GetService<ILocalizationManager>().GetLocalizedMessage(ClickingKey), Is.EqualTo(ClickingValueEn));
        }

        [Test]
        public void Should_BePossibleTo_UseLocalizationManager_ForUnknownKey()
        {
            var unknownKey = "loc.unknown.fake.key";
            Assert.That(ServiceProvider.GetService<ILocalizationManager>().GetLocalizedMessage(unknownKey), Is.EqualTo(unknownKey));
        }

        [Test]
        public void Should_ReturnNonKeyValues_AndNotEmptyValues_ForKeysWithoutParams([ValueSource(nameof(SupportedLanguages))] string language, [ValueSource(nameof(KeysWithoutParams))] string key)
        {
            var configuration = new DynamicConfiguration
            {
                Language = language
            };
            var localizedValue = new LocalizationManager(configuration, Logger.Instance).GetLocalizedMessage(key);
            Assert.That(localizedValue, Is.Not.EqualTo(key), "Value should be defined in resource files");
            Assert.That(localizedValue, Is.Not.Empty, "Value should not be empty");
        }

        [Test]
        public void Should_ReturnNonKeyValue_ForKeysPresentInCore_IfLanguageMissedInSiblingAssembly()
        {
            var configuration = new DynamicConfiguration
            {
                Language = "en"
            };
            var localizedValue = new LocalizationManager(configuration, Logger.Instance, GetType().Assembly).GetLocalizedMessage(ClickingKey);

            Assert.That(localizedValue, Is.EqualTo(ClickingValueEn), "Value should match to expected");
        }

        [Test]
        public void Should_ReturnNonKeyValue_ForKeysPresentInCore_IfKeyMissedInSiblingAssembly()
        {

            var configuration = new DynamicConfiguration
            {
                Language = "be"
            };
            var localizedValue = new LocalizationManager(configuration, Logger.Instance, GetType().Assembly).GetLocalizedMessage(ClickingKey);

            Assert.That(localizedValue, Is.EqualTo(ClickingValueBe), "Value should match to expected");
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
            Assert.That(localizedValue, Is.Not.EqualTo(key), "Value should be defined in resource files");
            Assert.That(localizedValue, Is.Not.Empty, "Value should not be empty");
            Assert.That(localizedValue.Contains(paramsArray[0]), "Value should contain at least first parameter");
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

            public bool LogPageSource => throw new NotImplementedException();
        }
    }
}
