using YomoneyApp.ViewModels.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountMain : ContentPage
    {
        AccountViewModel viewModel;
        string HostDomain = "http://192.168.100.150:5000";
        string webviewLink = "/Mobile/Forms?SupplierId=5-0001-0001052&serviceId=1&ActionId=3&FormNumber=9&Customer=263784607691&CallType=FirstTime";
        string title = "Agent Sign Up";

        public AccountMain()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);

            ButtonJoin.Clicked += async (sender, e) =>
            {
                //await Navigation.PushAsync(new NewAccount());

                await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + webviewLink, title, false, null, false));

                //await Navigation.PushAsync(new WaletServices("", "", "", ""));
            };

            ButtonLogin.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new SignIn());
            };
        }
    }
}