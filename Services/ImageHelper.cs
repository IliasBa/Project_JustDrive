using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Project_JustDrive.Services
{
    public static class ImageHelper
    {
        public static BitmapImage LoadFromBytes(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            try
            {
                var bitmap = new BitmapImage();
                using (var stream = new System.IO.MemoryStream(data))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                return bitmap;
            }
            catch { return null; }
        }

        public static byte[] LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            try { return System.IO.File.ReadAllBytes(filePath); }
            catch { return null; }
        }
    }
}