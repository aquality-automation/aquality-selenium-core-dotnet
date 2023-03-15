using OpenQA.Selenium;
using SkiaSharp;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Selenium screenshot extensions.
    /// </summary>
    public static class ScreenshotExtensions
    {
        /// <summary>
        /// Represents given screenshot as an image.
        /// </summary>
        /// <param name="screenshot">Given screenshot.</param>
        /// <returns>Image.</returns>
        public static SKImage AsImage(this Screenshot screenshot)
        {
            return SKImage.FromEncodedData(screenshot.AsByteArray);
        }
    }
}
