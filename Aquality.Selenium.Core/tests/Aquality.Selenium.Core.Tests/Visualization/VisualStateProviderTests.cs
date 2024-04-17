using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Waitings;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using NUnit.Framework;
using OpenQA.Selenium;
using SkiaSharp;
using System;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;

namespace Aquality.Selenium.Core.Tests.Visualization
{
    public sealed class VisualStateProviderTests : TestWithBrowser
    {
        private static readonly Uri DynamicLoadingUrl = new($"{TestSite}/dynamic_loading/1");
        private static readonly Label StartLabel = new(By.XPath("//*[@id='start']//button"), "start", ElementState.Displayed);
        private static readonly Label LoadingLabel = new(By.Id("loading"), "loading", ElementState.Displayed);

        [SetUp]
        public new void SetUp()
        {
            GoToUrl(DynamicLoadingUrl);
        }

        private static void StartLoading()
        {
            StartLabel.Click();
        }

        [Test]
        public void Should_BePossibleTo_GetElementSize()
        {
            Size size = Size.Empty;
            Assert.DoesNotThrow(() => size = StartLabel.Visual.Size);
            Assert.That(size.IsEmpty, Is.False);
        }

        [Test]
        public void Should_BePossibleTo_GetElementLocation()
        {
            Point location = Point.Empty;
            Assert.DoesNotThrow(() => location = StartLabel.Visual.Location);
            Assert.That(location.IsEmpty, Is.False);
        }

        [Test]
        public void Should_BePossibleTo_GetElementImage()
        {
            SKImage image = null;
            Assert.DoesNotThrow(() => image = StartLabel.Visual.Image);
            Assert.That(image, Is.Not.Null);
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
            AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>().WaitFor(() => firstImage.Height < LoadingLabel.Visual.Size.Height);
            Assert.Multiple(() =>
            {
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0), Is.Not.EqualTo(0));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.2f), Is.AtMost(0.3));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.4f), Is.AtMost(0.2));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.6f), Is.AtMost(0.1));
                Assert.That(LoadingLabel.Visual.GetDifference(firstImage, threshold: 0.8f), Is.EqualTo(0));
            });
        }
    }
}
