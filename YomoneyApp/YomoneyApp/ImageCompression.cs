using System;
using System.Collections.Generic;
using System.Text;
using YomoneyApp.Interfaces;

namespace YomoneyApp
{
    public class ImageCompression : IImageCompressionService
    {
        public ImageCompression() { }

        public byte[] CompressImage(byte[] imageData, string destinationPath, int compressionPercentage)
        {
            var resizedImage = GetResizedImage(byte[] imageData, int compressionPercentage);
          
        }

        private byte[] GetResizedImage(byte[] imageData, int compressionPercentage)
        {
            System.Drawing.Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
        }
    }
}
