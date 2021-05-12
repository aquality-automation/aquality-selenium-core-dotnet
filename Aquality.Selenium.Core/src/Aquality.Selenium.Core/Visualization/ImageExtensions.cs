using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Image resizing, gray-scaling and comparison extensions
    /// Special thanks to https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET
    /// </summary>
    public static class ImageExtensions
    {
        private const int ComparisonWidth = 16; //todo: to be got from configuration
        private const int ComparisonHeight = 16;
        private const int ThresholdDivisor = 255;
        private const float DefaultThreshold = 3f / ThresholdDivisor;
        private static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <param name="threshold">How big a difference will be ignored as a percentage - value between 0 and 1, the default is 3/255.</param>
        /// <returns>The difference between the two images as a percentage  - value between 0 and 1.</returns>
        /// <remarks>See http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale for more details</remarks>
        public static float PercentageDifference(this Image thisOne, Image theOtherOne, float threshold = DefaultThreshold)
        {
            if (threshold < 0 || threshold > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold), threshold, "Threshold should be between 0 and 1");
            }

            var byteThreshold = Convert.ToByte(threshold * ThresholdDivisor);
            return thisOne.PercentageDifference(theOtherOne, byteThreshold);
        }

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        /// <remarks>See http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale for more details</remarks>
        private static float PercentageDifference(this Image thisOne, Image theOtherOne, byte threshold = 3)
        {
            var differences = thisOne.GetDifferences(theOtherOne);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / ((float) ComparisonWidth * ComparisonHeight);
        }

        /// <summary>
        /// Finds the differences between two images and returns them in a double-array
        /// </summary>
        /// <param name="thisOne">The first image</param>
        /// <param name="theOtherOne">The image to compare with</param>
        /// <returns>the differences between the two images as a double-array</returns>
        private static byte[,] GetDifferences(this Image thisOne, Image theOtherOne)
        {
            var firstGray = thisOne.GetResizedGrayScaleValues();
            var secondGray = theOtherOne.GetResizedGrayScaleValues();

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
        /// Resizes an image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static Image Resize(this Image originalImage, int newWidth, int newHeight)
        {
            var smallVersion = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(smallVersion))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return smallVersion;
        }

        /// <summary>
        /// Converts an image to gray scale
        /// </summary>
        /// <param name="original">The image to gray scale</param>
        /// <returns>A gray scale version of the image</returns>
        /// <remarks>See http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale for more details</remarks>
        public static Image GetGrayScaleVersion(this Image original)
        {
            //create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (var graphics = Graphics.FromImage(newBitmap))
            {
                //create some image attributes
                var attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(ColorMatrix);

                //draw the original image on the new image
                //using the gray-scale color matrix
                graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return newBitmap;
        }

        /// <summary>
        /// Gets the lightness of the image in sections (by default 256 sections, 16x16)
        /// </summary>
        /// <param name="img">The image to get the lightness for</param>
        /// <returns>A double-array (16x16 by default) containing the lightness of the sections(256 by default)</returns>
        private static byte[,] GetResizedGrayScaleValues(this Image img)
        {
            using (var thisOne = (Bitmap)img.Resize(ComparisonWidth, ComparisonHeight).GetGrayScaleVersion())
            {
                byte[,] grayScale = new byte[thisOne.Width, thisOne.Height];

                for (int y = 0; y < thisOne.Height; y++)
                {
                    for (int x = 0; x < thisOne.Width; x++)
                    {
                        grayScale[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R);
                    }
                }

                return grayScale;
            }
        }
    }
}
