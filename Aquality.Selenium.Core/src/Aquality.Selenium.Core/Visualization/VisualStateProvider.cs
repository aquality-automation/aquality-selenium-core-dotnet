using Aquality.Selenium.Core.Logging;
using Aquality.Selenium.Core.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Aquality.Selenium.Core.Visualization
{
    public class VisualStateProvider : IVisualStateProvider
    {
        private readonly IImageComparator imageComparator;
        private readonly IElementActionRetrier actionRetrier;
        private readonly Func<RemoteWebElement> getElement;
        private readonly LogVisualState logVisualState;

        public VisualStateProvider(IImageComparator imageComparator, IElementActionRetrier actionRetrier, Func<RemoteWebElement> getElement, LogVisualState logVisualState)
        {
            this.imageComparator = imageComparator;
            this.actionRetrier = actionRetrier;
            this.getElement = getElement;
            this.logVisualState = logVisualState;
        }

        public Size Size => GetLoggedValue(nameof(Size), element => element.Size);

        public Point Location => GetLoggedValue(nameof(Location), element => element.Location);

        public Image Image 
            => GetLoggedValue(nameof(Image), element => element.GetScreenshot().AsImage(), image => image.Size.ToString(), new[] { typeof(WebDriverException) });

        private T GetLoggedValue<T>(string name, Func<RemoteWebElement, T> getValue, Func<T, string> toString = null, IEnumerable<Type> handledExceptions = null)
        {
            logVisualState($"loc.el.visual.get{name.ToLower()}");
            var value = actionRetrier.DoWithRetry(() => getValue(getElement()), handledExceptions);
            logVisualState($"loc.el.visual.{name.ToLower()}.value", toString == null ? value.ToString() : toString(value));
            return value;
        }

        public float GetDifference(Image theOtherOne, float? threshold = null)
        {
            var currentImage = Image;
            if (threshold == null)
            {
                logVisualState("loc.el.visual.getdifference", theOtherOne.Size.ToString());
            }
            else
            {
                logVisualState("loc.el.visual.getdifference.withthreshold", theOtherOne.Size.ToString(), threshold?.ToString("P", CultureInfo.InvariantCulture));
            }
            var value = imageComparator.PercentageDifference(currentImage, theOtherOne, threshold);
            logVisualState("loc.el.visual.difference.value", value.ToString("P", CultureInfo.InvariantCulture));
            return value;
        }
    }
}
