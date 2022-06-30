using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace YomoneyApp.Interfaces
{
    public interface IMyOwnNetService
    {
        HttpClientHandler GetHttpClientHandler();
    }
}
