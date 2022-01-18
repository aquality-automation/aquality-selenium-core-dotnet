using System;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Aquality.Selenium.Core.Visualization
{
    public sealed class ImageFormat
    {
        private readonly string format;

        public static readonly ImageFormat Bmp = new ImageFormat(DrawingImageFormat.Bmp);
        public static readonly ImageFormat Emf = new ImageFormat(DrawingImageFormat.Emf);
        public static readonly ImageFormat Exif = new ImageFormat(DrawingImageFormat.Exif);
        public static readonly ImageFormat Gif = new ImageFormat(DrawingImageFormat.Gif);
        public static readonly ImageFormat Icon = new ImageFormat(DrawingImageFormat.Icon);
        public static readonly ImageFormat Jpg = new ImageFormat("Jpg");
        public static readonly ImageFormat Jpeg = new ImageFormat(DrawingImageFormat.Jpeg);
        public static readonly ImageFormat MemoryBmp = new ImageFormat(DrawingImageFormat.MemoryBmp);
        public static readonly ImageFormat Png = new ImageFormat(DrawingImageFormat.Png);
        public static readonly ImageFormat Tif = new ImageFormat("Tif");
        public static readonly ImageFormat Tiff = new ImageFormat(DrawingImageFormat.Tiff);
        public static readonly ImageFormat Wmf = new ImageFormat(DrawingImageFormat.Wmf);

        private ImageFormat(string format) { this.format = format; }
        private ImageFormat(DrawingImageFormat format) { this.format = format.ToString(); }

        /// <summary>
        /// Convert an image format from string representation to ImageFormat
        /// </summary>
        /// <param name="format">String representation of image format</param>
        /// <returns>ImageFormat version of string representation</returns>
        public static ImageFormat Convert(string format)
        {
            switch (format.ToLower())
            {
                case ".bmp":
                    return Bmp;
                case ".emf":
                    return Emf;
                case ".exif":
                    return Exif;
                case ".gif":
                    return Gif;
                case ".icon":
                    return Icon;
                case ".jpg":
                    return Jpg;
                case ".jpeg":
                    return Jpeg;
                case ".memorybmp":
                    return MemoryBmp;
                case ".png":
                    return Png;
                case ".tif":
                    return Tif;
                case ".tiff":
                    return Tiff;
                case ".wmf":
                    return Wmf;
                default:
                    throw new NotSupportedException($"Unknown <{format}> extension for image file");
            }
        }

        /// <summary>
        /// Overridden method to convert an ImageFormat to string representation
        /// </summary>
        /// <returns>string version of ImageFormat representation</returns>
        public override string ToString() => $".{format.ToLower()}";
    }
}
