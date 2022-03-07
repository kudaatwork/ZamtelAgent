using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YomoneyApp.Interfaces
{
    internal interface IApiService
    {
        Task UploadImageAsync(Stream image, string fileName);
    }
}
