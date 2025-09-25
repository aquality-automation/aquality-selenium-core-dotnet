using Aquality.Selenium.Core.Elements;
using Aquality.Selenium.Core.Waitings;
using Aquality.Selenium.Core.Tests.Applications.Browser;
using Aquality.Selenium.Core.Tests.Applications.Browser.Elements;
using Aquality.Selenium.Core.Visualization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using Aquality.Selenium.Core.Utilities;
using SkiaSharp;
using System.Collections.Generic;

namespace Aquality.Selenium.Core.Tests.Visualization
{
    public sealed class ImageComparatorTests : TestWithBrowser
    {
        private static readonly Uri DynamicLoadingUrl = new($"{TestSite}/dynamic_loading/1");
        private static readonly Label StartLabel = new(By.XPath("//*[@id='start']//button"), "start", ElementState.Displayed);
        private static readonly Label LoadingLabel = new(By.Id("loading"), "loading", ElementState.Displayed);

        private IImageComparator ImageComparator => AqualityServices.ServiceProvider.GetRequiredService<IImageComparator>();

        [SetUp]
        public new void SetUp()
        {
            GoToUrl(DynamicLoadingUrl);
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
            Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForSameElement_WithZeroThreshold()
        {
            var firstImage = StartLabel.GetElement().GetScreenshot().AsImage();
            var secondImage = StartLabel.GetElement().GetScreenshot().AsImage();
            Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0), Is.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForDifferentElements()
        {
            var firstImage = StartLabel.GetElement().GetScreenshot().AsImage();
            StartLoading();
            var secondImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage), Is.Not.EqualTo(0));
        }

        [Test]
        public void Should_BePossibleTo_GetPercentageDifference_ForDifferentElements_WithFullThreshold()
        {
            var firstImage = StartLabel.GetElement().GetScreenshot().AsImage();
            StartLoading();
            var secondImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 1), Is.EqualTo(0));
        }

        [Test, Retry(5)]
        public void Should_BePossibleTo_GetPercentageDifference_ForSimilarElements()
        {
            SKImage firstImage = null, secondImage = null;
            AqualityServices.ServiceProvider.GetRequiredService<IActionRetrier>().DoWithRetry(() =>
            {
                AqualityServices.Application.Driver.Navigate().Refresh();
                StartLoading();
                firstImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
                AqualityServices.ServiceProvider.GetRequiredService<IConditionalWait>().WaitFor(() => firstImage.Height < LoadingLabel.Visual.Size.Height);
                secondImage = LoadingLabel.GetElement().GetScreenshot().AsImage();
            }, new List<Type> { typeof(WebDriverException)});

            Assert.Multiple(() =>
            {
                Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0), Is.Not.EqualTo(0));
                Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0.2f), Is.AtMost(0.3));
                Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0.4f), Is.AtMost(0.2));
                Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0.6f), Is.AtMost(0.1));
                Assert.That(ImageComparator.PercentageDifference(firstImage, secondImage, threshold: 0.8f), Is.EqualTo(0));
            });
        }
    }
}
