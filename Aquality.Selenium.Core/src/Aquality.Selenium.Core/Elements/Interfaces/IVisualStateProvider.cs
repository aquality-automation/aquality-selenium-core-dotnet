using Aquality.Selenium.Core.Configurations;
using Aquality.Selenium.Core.Visualization;
using System.Drawing;

namespace Aquality.Selenium.Core.Elements.Interfaces
{
    /// <summary>
    /// Provides visual state of the element.
    /// </summary>
    public interface IVisualStateProvider
    {
        /// <summary>
        /// Gets a size object containing the height and width of this element.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Gets a point object containing the coordinates of the upper-left 
        /// corner of this element relative to the upper-left corner of the page.
        /// </summary>
        Point Location { get; }

        /// <summary>
        /// Gets an image containing the screenshot of the element.
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// Gets the difference between the image of the element and the provided image
        /// using <see cref="IImageComparator"/>
        /// </summary>
        /// <param name="theOtherOne">The image to compare the element with.</param>
        /// <param name="threshold">How big a difference will be ignored as a percentage - value between 0 and 1.
        /// If the value is null, the default value is got from <see cref="IVisualizationConfiguration"/>.</param>
        /// <returns>The difference between the two images as a percentage  - value between 0 and 1.</returns>
        float GetDifference(Image theOtherOne, float? threshold = null);
    }
}
