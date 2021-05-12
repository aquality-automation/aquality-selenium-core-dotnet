using OpenQA.Selenium;
using System.Drawing;
using System.IO;

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
        public static Image AsImage(this Screenshot screenshot)
        {
            return Image.FromStream(new MemoryStream(screenshot.AsByteArray));
        }
    }
}
