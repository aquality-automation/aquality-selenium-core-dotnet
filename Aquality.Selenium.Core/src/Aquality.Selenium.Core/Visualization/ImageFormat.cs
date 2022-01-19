using System;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Aquality.Selenium.Core.Visualization
{
    public sealed class ImageFormat
    {
        public string Extension { get; }
        public DrawingImageFormat Format { get; }

        public static readonly ImageFormat Bmp = new ImageFormat(DrawingImageFormat.Bmp);
        public static readonly ImageFormat Emf = new ImageFormat(DrawingImageFormat.Emf);
        public static readonly ImageFormat Exif = new ImageFormat(DrawingImageFormat.Exif);
        public static readonly ImageFormat Gif = new ImageFormat(DrawingImageFormat.Gif);
        public static readonly ImageFormat Icon = new ImageFormat(".ico", DrawingImageFormat.Icon);
        public static readonly ImageFormat Jpg = new ImageFormat(".jpg", DrawingImageFormat.Jpeg);
        public static readonly ImageFormat Jpeg = new ImageFormat(DrawingImageFormat.Jpeg);
        public static readonly ImageFormat MemoryBmp = new ImageFormat(DrawingImageFormat.MemoryBmp);
        public static readonly ImageFormat Png = new ImageFormat(DrawingImageFormat.Png);
        public static readonly ImageFormat Tif = new ImageFormat(".tif", DrawingImageFormat.Tiff);
        public static readonly ImageFormat Tiff = new ImageFormat(DrawingImageFormat.Tiff);
        public static readonly ImageFormat Wmf = new ImageFormat(DrawingImageFormat.Wmf);

        private ImageFormat(DrawingImageFormat format) : this($".{format.ToString().ToLower()}", format) { }
        private ImageFormat(string extension, DrawingImageFormat format) 
        { 
            Extension = extension;
            Format = format;
        }

        /// <summary>
        /// Parse an image format from string representation to ImageFormat
        /// </summary>
        /// <param name="format">String representation of image format</param>
        /// <returns>ImageFormat version of string representation</returns>
        public static ImageFormat Parse(string format)
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
                case ".ico":
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
    }
}
