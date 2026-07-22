using System;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace NewLab.Helpers
{
    public static class BarcodeImageGenerator
    {
        public static BitmapSource GenerateCode128(string data, int widthPx = 300, int heightPx = 100)
        {
            var writer = new BarcodeWriter<System.Drawing.Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = widthPx,
                    Height = heightPx,
                    Margin = 5
                },
                Renderer = new BitmapRenderer()
            };

            using var bitmap = writer.Write(data);
            return ConvertToBitmapSource(bitmap);
        }

        private static BitmapSource ConvertToBitmapSource(System.Drawing.Bitmap bmp)
        {
            var hBitmap = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.Release(hBitmap);
            }
        }
    }
}
