using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

#if __ANDROID__
using Android.Graphics;
#endif

namespace YomoneyApp
{
    public static class ImageResizer
    {
        static ImageResizer()
        {
        }

        public static async Task<byte[]> ResizeImage(byte[] imageData, float width, float height)
        {
            var platform = Device.RuntimePlatform;
            switch (platform)
            {
                case Device.Android:
                    return ResizeImageAndroid(imageData, width, height);
                case Device.iOS:
                    return ResizeImageIOS(imageData, width, height);
                default:
                    break;
            }

            return null;
        }

        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }

        public static byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;
            //create a 24bit RGB image
            using (CoreGraphics.CGBitmapContext context = new CoreGraphics.CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CoreGraphics.CGColorSpace.CreateDeviceRGB(),
                                                 CoreGraphics.CGImageAlphaInfo.PremultipliedFirst))
            {
                RectangleF imageRect = new RectangleF(0, 0, width, height);
                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);
                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);
                // save the image as a jpeg
                return resizedImage.AsJPEG().ToArray();
            }
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
    }
}
