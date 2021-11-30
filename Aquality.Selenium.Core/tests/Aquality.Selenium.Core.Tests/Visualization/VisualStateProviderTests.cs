using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Visualization;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Aquality.Selenium.Core.Tests.Localization
{
    public sealed class VisualStateProviderTests : TestWithBrowser
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
        public void Should_BePossibleTo_GetElementSize()
        {
            Size size = Size.Empty;
            Assert.DoesNotThrow(() => size = StartLabel.Visual.Size);
            Assert.False(size.IsEmpty);
        }

        [Test]
        public void Should_BePossibleTo_GetElementLocation()
        {
            Point location = Point.Empty;
            Assert.DoesNotThrow(() => location = StartLabel.Visual.Location);
            Assert.False(location.IsEmpty);
        }

        [Test]
        public void Should_BePossibleTo_GetElementImage()
        {
            Image image = null;
            Assert.DoesNotThrow(() => image = StartLabel.Visual.Image);
            Assert.IsNotNull(image);
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSameElement()
        {
            var firstImage = StartLabel.Visual.Image;
            Assert.That(StartLabel.Visual.GetDifference(firstImage), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSameElement_WithZeroThreshold()
        {
            var firstImage = StartLabel.Visual.Image;
            Assert.That(StartLabel.Visual.GetDifference(firstImage, threshold: 0), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForDifferentElements()
        {
            var firstImage = StartLabel.Visual.Image;
            StartLoading();
            Assert.That(LoadingLabel.Visual.GetDifference(firstImage), Is.Not.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForDifferentElements_WithFullThreshold()
        {
            var firstImage = StartLabel.Visual.Image;
            StartLoading();
            Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 1), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSimilarElements()
        {
            StartLoading();
            var firstImage = LoadingLabel.Visual.Image;
            var secondImage = ((ITakesScreenshot)LoadingLabel.GetElement()).GetScreenshot().AsImage();
            Assert.Multiple(() =>
            {
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0), Is.Not.EqualTo(0));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.2f), Is.AtMost(0.2));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.4f), Is.AtMost(0.15));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.6f), Is.AtMost(0.1));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.8f), Is.EqualTo(0));
            });
        }
    }
}
