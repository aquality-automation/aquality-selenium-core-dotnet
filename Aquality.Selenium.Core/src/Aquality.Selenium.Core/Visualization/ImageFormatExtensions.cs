using System;
using System.Drawing.Imaging;

namespace Aquality.Selenium.Core.Visualization
{
    /// <summary>
    /// Image format conversion extensions.
    /// </summary>
    public static class ImageFormatExtensions
    {

        /// <summary>
        /// Convert an image format from string representation to ImageFormat
        /// </summary>
        /// <param name="format">String representation of image format</param>
        /// <returns>ImageFormat version of string representation</returns>
        public static ImageFormat Convert(string format)
        {
            switch (format.ToLower())
            {
                case ".png":
                    return ImageFormat.Png;
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".icon":
                    return ImageFormat.Icon;
                case ".gif":
                    return ImageFormat.Gif;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".exif":
                    return ImageFormat.Exif;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                default:
                    throw new NotSupportedException($"Unknown <{format}> extension for image file");
            }
        }

        /// <summary>
        /// Convert an image format from ImageFormat representation to string
        /// </summary>
        /// <param name="format">ImageFormat representation of image format</param>
        /// <returns>string version of ImageFormat representation</returns>
        public static string Convert(ImageFormat format) => $".{format.ToString().ToLower()}";
    }
}
