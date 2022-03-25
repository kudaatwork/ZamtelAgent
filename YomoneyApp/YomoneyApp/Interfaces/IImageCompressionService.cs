using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Interfaces
{
    public interface IImageCompressionService
    {
        byte[] CompressImage(byte[] imageData, string destinationPath, int compressionPercentage);   
    }
}
