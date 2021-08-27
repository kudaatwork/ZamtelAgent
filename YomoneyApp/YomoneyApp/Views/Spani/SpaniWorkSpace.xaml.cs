using YomoneyApp.Services;
using YomoneyApp.Views.Chat;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Webview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpaniWorkSpace : TabbedPage
    {
       
        RequestViewModel viewModel;
       
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public SpaniWorkSpace()
        {
            InitializeComponent();
           /* 
            var Profile = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    AccessSettings acnt = new AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;
                    Navigation.PushAsync(new WebviewPage("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
                }),
                Text = "My Profile",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };
            var request = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();
                    px.Title = "Profile Image";
                    Navigation.PushAsync(new FileUploadPage(px));
                }),
                Text = "Profile Picture",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            ToolbarItems.Add(Profile);
            ToolbarItems.Add(request);
             */
        }
    }
}