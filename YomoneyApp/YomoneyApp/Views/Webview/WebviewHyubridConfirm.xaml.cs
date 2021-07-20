using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Views.TemplatePages;

namespace YomoneyApp.Views.Webview
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebviewHyubridConfirm : ContentPage
    {
        public WebviewHyubridConfirm()
        {
            InitializeComponent();
            var hybridWebView = new HybridWebView
            {
                Uri = "index.html",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));
            Padding = new Thickness(0, 20, 0, 0);
            Content = hybridWebView;
        }
    }
}