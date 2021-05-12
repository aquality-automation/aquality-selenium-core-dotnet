using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.WindowsApp.Elements;
using Aquality.Selenium.Core.Visualization;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Aquality.Selenium.Core.Tests.Localization
{
    public sealed class VisualizationTests : TestWithBrowser
    {
        private static readonly Uri DynamicLoadingUrl = new Uri($"{TestSite}/dynamic_loading/1");
        private static readonly Label StartLabel = new Label(By.XPath("//*[@id='start']//button"), "start", ElementState.Displayed);
        private static readonly Label LoadingLabel = new Label(By.Id("loading"), "loading", ElementState.Displayed);

        [SetUp]
        public new void SetUp()
        {
            AqualityServices.Application.Driver.Navigate().GoToUrl(DynamicLoadingUrl);
        }

        private void StartLoading()
        {
            StartLabel.Click();
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSameElement()
        {
            var firstImage = StartLabel.GetElement().GetScreenshot().AsImage();
            var secondImage = StartLabel.GetElement().GetScreenshot().AsImage();
            Assert.That(firstImage.PercentageDifference(secondImage), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForDifferentElements()
        {
            var firstImage = StartLabel.GetElement().GetScreenshot().AsImage();
            StartLoading();
            var secondImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            Assert.That(firstImage.PercentageDifference(secondImage), Is.Not.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSimilarElements()
        {
            StartLoading();
            var firstImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            var secondImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            Assert.Multiple(() =>
            {
                Assert.That(firstImage.PercentageDifference(secondImage, threshold: 0), Is.Not.EqualTo(0));
                Assert.That(firstImage.PercentageDifference(secondImage, threshold: 0.2f), Is.AtMost(0.2));
                Assert.That(firstImage.PercentageDifference(secondImage, threshold: 0.4f), Is.AtMost(0.1));
                Assert.That(firstImage.PercentageDifference(secondImage, threshold: 0.8f), Is.EqualTo(0));
            });
        }
    }
}
