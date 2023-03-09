using SkiaSharp;
using System.Drawing;
using System.IO;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Image resizing and gray-scaling extensions.
    /// Special thanks to https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET
    /// </summary>
    public static class ImageExtensions
    {
        private static readonly SKColorFilter ColorMatrix = SKColorFilter.CreateColorMatrix(new float[]
        {
            0.21f, 0.72f, 0.07f, 0, 0,
            0.21f, 0.72f, 0.07f, 0, 0,
            0.21f, 0.72f, 0.07f, 0, 0,
            0,     0,     0,      1, 0
        });

        /// <summary>
        /// Resizes an image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static SKImage Resize(this SKImage originalImage, int newWidth, int newHeight)
        {
            using (var srcBitmap = SKBitmap.FromImage(originalImage))
            {
                var resizeInfo = new SKImageInfo(newWidth, newHeight);
                using (var smallVersion = srcBitmap.Resize(resizeInfo, SKFilterQuality.High))
                {
                    return SKImage.FromBitmap(smallVersion);
                }
            }                
        }

        /// <summary>
        /// Converts an image to gray scale
        /// </summary>
        /// <param name="original">The image to gray scale</param>
        /// <returns>A gray scale version of the image</returns>
        /// <remarks>See http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale for more details</remarks>
        public static SKImage GetGrayScaleVersion(this SKImage original)
        {
            //create a blank bitmap the same size as original
            var info = new SKImageInfo(original.Width, original.Height);
            var newBitmap = new SKBitmap(info);
            

            //create some image attributes
            using (var paint = new SKPaint())
            //get a graphics object from the new image
            using (var graphics = new SKCanvas(newBitmap))
            {
                //set the color matrix attribute
                paint.ColorFilter = ColorMatrix;                

                //draw the original image on the new image
                //using the gray-scale color matrix
                graphics.DrawImage(original, 0, 0, paint);
            }

            return SKImage.FromBitmap(newBitmap);
        }

        public static Size Size(this SKImage image) => new Size(image.Width, image.Height);

        public static void Save(this SKImage image, string name, SKEncodedImageFormat format = SKEncodedImageFormat.Png)
        {
            using (Stream stream = new FileStream(name, FileMode.OpenOrCreate))
            {
                SKData d = image.Encode(format, 100);
                d.SaveTo(stream);
            }
        }
    }
}
