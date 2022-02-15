using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.ActiveCountry;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Menus;
using YomoneyApp.Views.Profile;
using YomoneyApp.Views.Profile.Loyalty;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaletServices : ContentPage
    {
        WalletServicesViewModel viewModel;        

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public WaletServices(string loyalty, string services, string tasks,string orders)
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.WalletServicesViewModel(this);
            Title = "Dashboard";
            if (string.IsNullOrEmpty(loyalty)) loyalty = "0";
            if (string.IsNullOrEmpty(services)) services = "0";
            if (string.IsNullOrEmpty(tasks)) tasks = "0";
            if (string.IsNullOrEmpty(orders))orders ="0";
            viewModel.LoyaltySchemes = loyalty;
            viewModel.Services = services;
            viewModel.Tasks = tasks;
            viewModel.Orders = orders;  
            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

           await viewModel.ExecuteGetStoresCommand();
           await viewModel.ExecuteGetDashboardItemsCommand();
           await viewModel.ExecuteCetAccountStatusCommand();
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string uname = acnt.UserName;
            string link = "https://www.yomoneyservice.com/Mobile/Projects?Id=" + uname;

            Navigation.PushAsync(new WebviewHyubridConfirm(link, "My Tasks", true, "#df782d",false));
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {

            MenuItem menuItem = new MenuItem();

            menuItem.Title = "My Services";
            menuItem.Image = "https://www.yomoneyservice.com/Content/Spani/Images/myServices.jpg";
            menuItem.Section = "Yomoney";
            menuItem.ServiceId = 11;
            menuItem.SupplierId = "All";
            menuItem.TransactionType = 1;

            Navigation.PushAsync(new ServiceProviders(menuItem));
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            MenuItem menuItem = new MenuItem();

            menuItem.Title = "My Services";
            menuItem.Image = "https://www.yomoneyservice.com/Content/Spani/Images/myServices.jpg";
            menuItem.Section = "Yomoney";
            menuItem.ServiceId = 11;
            menuItem.SupplierId = "All";
            menuItem.TransactionType = 1;

            Navigation.PushAsync(new ServiceProviders(menuItem));
        }

        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string uname = acnt.UserName;
            string link = "https://www.yomoneyservice.com/Mobile/Projects?Id=" + uname;

            Navigation.PushAsync(new WebviewHyubridConfirm(link, "My Tasks", true, "#df782d",false));
        }

        private void TapGestureRecognizer_Tapped_4(object sender, EventArgs e)
        {
            MenuItem menuItem = new MenuItem();

            menuItem.Title = "Loyalty Points";
            menuItem.Image = "https://www.yomoneyservice.com/Content/Spani/Images/Loyalty.jpg";
            menuItem.Section = "Loyalty";
            menuItem.ServiceId = 1;
            menuItem.SupplierId = "All";
            menuItem.TransactionType = 6;

            Navigation.PushAsync(new SelectPage(menuItem));
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            MenuItem menuItem = new MenuItem();

            menuItem.Title = "Loyalty Points";
            menuItem.Image = "https://www.yomoneyservice.com/Content/Spani/Images/Loyalty.jpg";
            menuItem.Section = "Loyalty";
            menuItem.ServiceId = 1;
            menuItem.SupplierId = "All";
            menuItem.TransactionType = 6;

            Navigation.PushAsync(new SelectPage(menuItem));
        }
        private void TapGestureRecognizer_Tapped_6(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string uname = acnt.UserName;
            string link = "https://www.yomoneyservice.com/Mobile/PurchaseOrders?Id=" + uname;

            Navigation.PushAsync(new WebviewHyubridConfirm(link, "Purchase Orders", true, "#df782d"));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PersonalDetails(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ActiveCountryPicker());
        }
    }
}