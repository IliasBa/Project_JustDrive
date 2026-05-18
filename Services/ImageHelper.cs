using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Project_JustDrive.Services
{
    public static class ImageHelper
    {
        public static string GetFilePath(string packPath)
        {
            if (string.IsNullOrEmpty(packPath)) return null;

            string fileName = Path.GetFileName(packPath);
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDir, "Images", fileName);

            return File.Exists(filePath) ? filePath : null;
        }

        public static BitmapImage LoadImage(string packPath)
        {
            try
            {
                string filePath = GetFilePath(packPath);
                if (string.IsNullOrEmpty(filePath)) return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch { return null; }
        }
    }
}