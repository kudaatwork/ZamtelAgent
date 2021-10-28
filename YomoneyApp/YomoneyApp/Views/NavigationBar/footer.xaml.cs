using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.TransactionHistory;
using YomoneyApp.Views.Webview;
using YomoneyApp.Views.Spani;

namespace YomoneyApp.Views.NavigationBar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class footer : ContentView
    {
        HomeViewModel viewModel;
        public footer()
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.HomeViewModel(null);

            btnHome.Clicked += async (sender, e) =>
            {
                var existingPages = Navigation.NavigationStack.ToList();

                int cnt = 2;
                foreach (var page in existingPages)
                {
                    if (cnt < existingPages.Count)
                    {
                        Navigation.RemovePage(page);
                    }
                    cnt++;
                }

                await Navigation.PushAsync(new HomePage());
            };

            btnTransactions.Clicked += async (sender, e) =>
            {
                var existingPages = Navigation.NavigationStack.ToList();

                int cnt = 2;

                foreach (var page in existingPages)
                {
                    if (cnt < existingPages.Count)
                    {
                        Navigation.RemovePage(page);
                    }
                    cnt++;
                }

                await Navigation.PushAsync(new Transactions());
            };

            btnProfile.Clicked += async (sender, e) =>
            {
                var existingPages = Navigation.NavigationStack.ToList();
                int cnt = 2;
                foreach (var page in existingPages)
                {
                    if (cnt < existingPages.Count)
                    {
                        Navigation.RemovePage(page);
                    }
                    cnt++;
                }

                AccessSettings acnt = new AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                string uri = "https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname;

               // await Navigation.PushAsync(new WebviewHyubridConfirm(uri));

                await Navigation.PushAsync(new WebviewPage(uri, "My Profile", false, null));
            };

            btnMyServices.Clicked += async (sender, e) =>
            {
                var existingPages = Navigation.NavigationStack.ToList();

                int cnt = 2;

                foreach (var page in existingPages)
                {
                    if (cnt < existingPages.Count)
                    {
                        Navigation.RemovePage(page);
                    }
                    cnt++;
                }

                await  Navigation.PushAsync(new WaletServices(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));
            };
        }
    }
}