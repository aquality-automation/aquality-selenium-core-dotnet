using Aquality.Selenium.Core.Configurations;
using SkiaSharp;
using System;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Compares images with defined threshold.
    /// Uses resizing and gray-scaling to simplify comparison.
    /// Uses parameters from <see cref="IVisualizationConfiguration"/>.
    /// Special thanks to https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET
    /// </summary>
    public class ImageComparator : IImageComparator
    {
        private const int ThresholdDivisor = 255;
        private readonly IVisualizationConfiguration visualizationConfiguration;

        public ImageComparator(IVisualizationConfiguration visualizationConfiguration)
        {
            this.visualizationConfiguration = visualizationConfiguration;
        }

        private float DefaultThreshold => visualizationConfiguration.DefaultThreshold;
        private int ComparisonHeight => visualizationConfiguration.ComparisonHeight;
        private int ComparisonWidth => visualizationConfiguration.ComparisonWidth;

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <param name="threshold">How big a difference will be ignored as a percentage - value between 0 and 1. 
        /// If the value is null, the default value is got from <see cref="IVisualizationConfiguration"/>.</param>
        /// <returns>The difference between the two images as a percentage  - value between 0 and 1.</returns>
        /// <remarks>See https://web.archive.org/web/20130208001434/http://tech.pro:80/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale for more details.</remarks>
        public virtual float PercentageDifference(SKImage thisOne, SKImage theOtherOne, float? threshold = null)
        {
            var thresholdValue = threshold ?? DefaultThreshold;
            if (thresholdValue < 0 || thresholdValue > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), thresholdValue, "Threshold should be between 0 and 1");
            }

            var byteThreshold = Convert.ToByte(thresholdValue * ThresholdDivisor);
            return PercentageDifference(thisOne, theOtherOne, byteThreshold);
        }

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        /// <remarks>See https://web.archive.org/web/20130208001434/http://tech.pro:80/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale for more details</remarks>
        protected virtual float PercentageDifference(SKImage thisOne, SKImage theOtherOne, byte threshold = 3)
        {
            var differences = GetDifferences(thisOne, theOtherOne);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / ((float)ComparisonWidth * ComparisonHeight);
        }

        /// <summary>
        /// Finds the differences between two images and returns them in a double-array
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <returns>the differences between the two images as a double-array</returns>
        protected virtual byte[,] GetDifferences(SKImage thisOne, SKImage theOtherOne)
        {
            var firstGray = GetResizedGrayScaleValues(thisOne);
            var secondGray = GetResizedGrayScaleValues(theOtherOne);

            var differences = new byte[ComparisonWidth, ComparisonHeight];
            for (int y = 0; y < ComparisonHeight; y++)
            {
                for (int x = 0; x < ComparisonWidth; x++)
                {
                    differences[x, y] = (byte)Math.Abs(firstGray[x, y] - secondGray[x, y]);
                }
            }

            return differences;
        }

        /// <summary>
        /// Gets the lightness of the image in sections (by default 256 sections, 16x16)
        /// </summary>
        /// <param name="img">The image to get the lightness for</param>
        /// <returns>A double-array (16x16 by default) containing the lightness of the sections(256 by default)</returns>
        protected virtual byte[,] GetResizedGrayScaleValues(SKImage img)
        {
            using (var thisOne = SKBitmap.FromImage(img.Resize(ComparisonWidth, ComparisonHeight).GetGrayScaleVersion()))
            {
                byte[,] grayScale = new byte[thisOne.Width, thisOne.Height];

                for (int y = 0; y < thisOne.Height; y++)
                {
                    for (int x = 0; x < thisOne.Width; x++)
                    {
                        grayScale[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).Red);
                    }
                }

                return grayScale;
            }
        }
    }
}
