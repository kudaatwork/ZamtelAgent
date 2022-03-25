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
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Login;

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

            //viewModel.ExecuteGetDashboardItemsCommand();

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

                //MenuItem menuItem = new MenuItem();

                //menuItem.Id = "1";
                //menuItem.Image = "https://www.yomoneyservice.com/Content/Logos/ZAMTEL/zamtel.png";
                //menuItem.Title = "SIM CARD MANAGEMENT";
                //menuItem.Description = "SIM CARD MANAGEMENT";
                //menuItem.Section = "Service";
                //menuItem.Note = "ZAMTEL";
                //menuItem.ServiceId = 1;
                //menuItem.TransactionType = 12;
                //menuItem.SupplierId = "5-0001-0001052";

                //await Navigation.PushAsync(new ServiceActions(menuItem));

                #region Load Services
                MenuItem menuItem = new MenuItem();

                menuItem.Id = "1";
                menuItem.Image = "https://www.yomoneyservice.com/Content/Logos/ZAMTEL/zamtel.png";
                menuItem.Title = "WAFAYA";
                menuItem.Note = "BANKING";
                menuItem.TransactionType = 12;
                menuItem.SupplierId = "5-0001-0001052";
                //menuItem.date = "0001-01-01T00:00:00";

                await Navigation.PushAsync(new ProviderServices(menuItem));
                #endregion
            };

            //btnTransactions.Clicked += async (sender, e) =>
            //{
            //    var existingPages = Navigation.NavigationStack.ToList();

            //    int cnt = 2;

            //    foreach (var page in existingPages)
            //    {
            //        if (cnt < existingPages.Count)
            //        {
            //            Navigation.RemovePage(page);
            //        }
            //        cnt++;
            //    }

            //    await Navigation.PushAsync(new Transactions());
            //};

            //btnProfile.Clicked += async (sender, e) =>
            //{
            //    var existingPages = Navigation.NavigationStack.ToList();
            //    int cnt = 2;
            //    foreach (var page in existingPages)
            //    {
            //        if (cnt < existingPages.Count)
            //        {
            //            Navigation.RemovePage(page);
            //        }
            //        cnt++;
            //    }

            //    AccessSettings acnt = new AccessSettings();
            //    string pass = acnt.Password;
            //    string uname = acnt.UserName;

            //    string uri = "https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname;

            //   // await Navigation.PushAsync(new WebviewHyubridConfirm(uri));

            //    await Navigation.PushAsync(new WebviewPage(uri, "My Profile", false, null));
            //};

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

                await  Navigation.PushAsync(new AccountMain());
            };
        }
    }
}