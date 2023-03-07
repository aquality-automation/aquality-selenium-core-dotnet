using Aquality.Selenium.Core.Configurations;
using SkiaSharp;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Compares images with defined threshold.
    /// Uses resizing and gray-scaling to simplify comparison.
    /// Default implementation uses parameters from <see cref="IVisualizationConfiguration"/>.
    /// </summary>
    public interface IImageComparator
    {
        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <param name="threshold">How big a difference will be ignored as a percentage - value between 0 and 1. </param>
        /// <returns>The difference between the two images as a percentage  - value between 0 and 1.</returns>
        float PercentageDifference(SKImage thisOne, SKImage theOtherOne, float? threshold = null);
    }
}
