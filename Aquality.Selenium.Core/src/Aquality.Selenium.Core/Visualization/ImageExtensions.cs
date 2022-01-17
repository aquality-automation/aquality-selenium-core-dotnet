using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Image resizing and gray-scaling extensions.
    /// Special thanks to https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET
    /// </summary>
    public static class ImageExtensions
    {
        private static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

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
    }
}
