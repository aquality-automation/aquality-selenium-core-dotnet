using OpenQA.Selenium;
using System.Drawing;
using System.IO;

namespace Aquality.Selenium.Core.Visualization
{
    public static class ScreenshotExtensions
    {
        public static Image AsImage(this Screenshot screenshot)
        {
            return Image.FromStream(new MemoryStream(screenshot.AsByteArray));
        }
    }
}
