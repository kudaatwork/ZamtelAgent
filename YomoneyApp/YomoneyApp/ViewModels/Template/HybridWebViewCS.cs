using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using YomoneyApp.Views.TemplatePages;

namespace YomoneyApp.ViewModels.Template
{
    public class HybridWebViewCS : ContentPage
    {
        public HybridWebViewCS()
        {
            var hybridWebView = new HybridWebView
            {
                Uri = "http://102.130.120.163:8087/Login/PosLoginM?username=Aqusales&password=admin"
            };
            
            hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));

            Padding = new Thickness(0, 0, 0, 0);
            Content = hybridWebView;
        }
    }
}
