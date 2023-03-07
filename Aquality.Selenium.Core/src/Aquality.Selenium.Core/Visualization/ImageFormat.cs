using SkiaSharp;
using System;

namespace Aquality.Selenium.Core.Visualization
{
    public sealed class ImageFormat
    {
        public string Extension { get; }
        public SKEncodedImageFormat Format { get; }

        public static readonly ImageFormat Bmp = new ImageFormat(SKEncodedImageFormat.Bmp);
        public static readonly ImageFormat Pkm = new ImageFormat(SKEncodedImageFormat.Pkm);
        public static readonly ImageFormat Webp = new ImageFormat(SKEncodedImageFormat.Webp);
        public static readonly ImageFormat Ktx = new ImageFormat(SKEncodedImageFormat.Ktx);
        public static readonly ImageFormat Avif = new ImageFormat(SKEncodedImageFormat.Avif);
        public static readonly ImageFormat Astc = new ImageFormat(SKEncodedImageFormat.Astc);
        public static readonly ImageFormat Dng = new ImageFormat(SKEncodedImageFormat.Dng);
        public static readonly ImageFormat Gif = new ImageFormat(SKEncodedImageFormat.Gif);
        public static readonly ImageFormat Ico = new ImageFormat(SKEncodedImageFormat.Ico);
        public static readonly ImageFormat Jpg = new ImageFormat(".jpg", SKEncodedImageFormat.Jpeg);
        public static readonly ImageFormat Jpeg = new ImageFormat(SKEncodedImageFormat.Jpeg);
        public static readonly ImageFormat Png = new ImageFormat(SKEncodedImageFormat.Png);
        public static readonly ImageFormat Heif = new ImageFormat(SKEncodedImageFormat.Heif);
        public static readonly ImageFormat Wbmp = new ImageFormat(SKEncodedImageFormat.Wbmp);

        private ImageFormat(SKEncodedImageFormat format) : this($".{format.ToString().ToLower()}", format) { }
        private ImageFormat(string extension, SKEncodedImageFormat format) 
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
                case ".heif":
                    return Heif;
                case ".avif":
                    return Avif;
                case ".gif":
                    return Gif;
                case ".ico":
                    return Ico;
                case ".jpg":
                    return Jpg;
                case ".jpeg":
                    return Jpeg;
                case ".wbmp":
                    return Wbmp;
                case ".png":
                    return Png;
                case ".astc":
                    return Astc;
                case ".dng":
                    return Dng;
                case ".ktx":
                    return Ktx;
                case ".webp":
                    return Webp;
                case ".pkm":
                    return Pkm;
                default:
                    throw new NotSupportedException($"Unknown <{format}> extension for image file");
            }
        }
    }
}
