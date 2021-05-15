using Aquality.Selenium.Core.Elements.Interfaces;
using Aquality.Selenium.Core.Utilities;
using Aquality.Selenium.Core.Visualization;
using OpenQA.Selenium.Remote;
using System;
using System.Drawing;
using System.Globalization;

namespace Aquality.Selenium.Core.Elements
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
            => GetLoggedValue(nameof(Image), element => element.GetScreenshot().AsImage(), image => image.Size.ToString());

        private T GetLoggedValue<T>(string name, Func<RemoteWebElement, T> getValue, Func<T, string> toString = null)
        {
            logVisualState($"loc.el.visual.get{name.ToLower()}");
            var value = actionRetrier.DoWithRetry(() => getValue(getElement()));
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
            var value = imageComparator.PercentageDifference(Image, theOtherOne, threshold);
            logVisualState("loc.el.visual.difference.value", value.ToString("P", CultureInfo.InvariantCulture));
            return value;
        }
    }
}
