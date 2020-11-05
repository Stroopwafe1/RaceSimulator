using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RaceSimulatorGUI {
    public static class ImageLoader {
        private static Dictionary<string, Bitmap> _imageCache;

        public static void Initialise() {
            _imageCache = new Dictionary<string, Bitmap>();
        }

        public static Bitmap GetImageFromCache(string imageURL) {
            if(_imageCache.ContainsKey(imageURL)) {
                return _imageCache[imageURL];
            }
            Bitmap image = new Bitmap(imageURL);
            _imageCache.Add(imageURL, image);
            return image;
        }

        public static Bitmap CreateEmptyBitmap(int width, int height) {
            Bitmap returnValue;
            if (_imageCache.ContainsKey("empty")) return (Bitmap)GetImageFromCache("empty").Clone();
            returnValue = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(returnValue);
            SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.Green);
            graphics.FillRectangle(solidBrush, 0, 0, width, height);
            _imageCache.Add("empty", returnValue);
            return (Bitmap)returnValue.Clone();
        }

        public static void ClearCache() {
            _imageCache?.Clear();
        }

        /// <summary>
        /// This is just for testing purposes
        /// </summary>
        /// <returns>Cache</returns>
        public static Dictionary<string, Bitmap> GetCache() {
            return _imageCache;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap) {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            } finally {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
