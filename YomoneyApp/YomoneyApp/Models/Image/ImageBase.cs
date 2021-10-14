using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models.Image
{
    public class ImageBase
    {
        private string base64;

        public ImageBase(string base64)
        {
            this.base64 = base64;
        }

        public string ImageBase64 { get; set; }
    }
}
