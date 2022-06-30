using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using YomoneyApp.Droid;
using YomoneyApp.Interfaces;

[assembly: Dependency(typeof(MyOwnNetService))]
namespace YomoneyApp.Droid
{
    internal class MyOwnNetService : IMyOwnNetService
    {
        public HttpClientHandler GetHttpClientHandler()
        {
            return new Xamarin.Android.Net.AndroidClientHandler();
        }
    }
}