using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using OpenQA.Selenium;
using SkiaSharp;
using System;
using System.Drawing;
using System.Globalization;

namespace Aquality.Selenium.Core.Visualization
{
    public class VisualStateProvider : IVisualStateProvider
    {
        private readonly IImageComparator imageComparator;
        private readonly IElementActionRetrier actionRetrier;
        private readonly Func<WebElement> getElement;
        private readonly LogVisualState logVisualState;

        public VisualStateProvider(IImageComparator imageComparator, IElementActionRetrier actionRetrier, Func<WebElement> getElement, LogVisualState logVisualState)
        {
            this.imageComparator = imageComparator;
            this.actionRetrier = actionRetrier;
            this.getElement = getElement;
            this.logVisualState = logVisualState;
        }

        public Size Size => GetLoggedValue(nameof(Size), element => element.Size);

        public Point Location => GetLoggedValue(nameof(Location), element => element.Location);

        public SKImage Image 
            => GetLoggedValue(nameof(Image), element => element.GetScreenshot().AsImage(), image => image?.Size().ToString());

        private T GetLoggedValue<T>(string name, Func<WebElement, T> getValue, Func<T, string> toString = null)
        {
            logVisualState($"loc.el.visual.get{name.ToLower()}");
            var value = actionRetrier.DoWithRetry(() => getValue(getElement()));
            logVisualState($"loc.el.visual.{name.ToLower()}.value", toString == null ? value.ToString() : toString(value));
            return value;
        }

        public float GetDifference(SKImage theOtherOne, float? threshold = null)
        {
            var currentImage = Image;
            float value = 1;

            if (threshold == null)
            {
                logVisualState("loc.el.visual.getdifference", theOtherOne.Size().ToString());
            }
            else
            {
                logVisualState("loc.el.visual.getdifference.withthreshold", theOtherOne.Size().ToString(), threshold?.ToString("P", CultureInfo.InvariantCulture));
            }

            if (currentImage != default)
            {
                value = imageComparator.PercentageDifference(currentImage, theOtherOne, threshold);
            }
            logVisualState("loc.el.visual.difference.value", value.ToString("P", CultureInfo.InvariantCulture));
            return value;
        }
    }
}
