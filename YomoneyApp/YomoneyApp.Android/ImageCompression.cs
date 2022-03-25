using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(YomoneyApp.Droid.ImageCompression))]
namespace YomoneyApp.Droid
{
    public class ImageCompression : Interfaces.IImageCompressionService
    {
        public ImageCompression() { }

        public byte[] CompressImage(byte[] imageData, string destinationPath, int compressionPercentage)
        {
            var resizedImage = GetResizedImage(imageData, compressionPercentage);

            var stream = new System.IO.FileStream(destinationPath, System.IO.FileMode.Open);
            stream.Write(resizedImage, 0, resizedImage.Length);
            stream.Flush();
            stream.Close();
            return resizedImage;
        }

        private byte[] GetResizedImage(byte[] imageData, int compressionPercentage)
        {
            Android.Graphics.Bitmap originalImage = Android.Graphics.BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                originalImage.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, compressionPercentage, ms);

                return ms.ToArray();
            }
        }
    }
}