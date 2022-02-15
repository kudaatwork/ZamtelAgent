using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views.Promotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyPromotions : ContentPage
    {
        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public MyPromotions(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new PromotionsViewModel(this, selected);
            SelectedItem = selected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.GetCurrentUserPromotions(SelectedItem);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //viewModel.IsBusy = true;
            //viewModel.Message = "Disabling Ad...";

            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem menuItem = new YomoneyApp.MenuItem();
                var menu = JsonConvert.SerializeObject(view.CommandParameter);
                menuItem = JsonConvert.DeserializeObject<MenuItem>(menu);

                PromotionsViewModel promotion = new PromotionsViewModel(this, menuItem);
                promotion.DisableAd(menuItem);

                //viewModel.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await DisplayAlert("Leads Error!", "Unable to gather leads from the server. Please check your internet connection and try again", "Ok");
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //viewModel.IsBusy = true;
            //viewModel.Message = "Deleting Ad";

            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem menuItem = new YomoneyApp.MenuItem();
                var menu = JsonConvert.SerializeObject(view.CommandParameter);
                menuItem = JsonConvert.DeserializeObject<MenuItem>(menu);

                PromotionsViewModel promotion = new PromotionsViewModel(this, menuItem);
                promotion.DeleteAd(menuItem);

                viewModel.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await DisplayAlert("Leads Error!", "Unable to gather leads from the server. Please check your internet connection and try again", "Ok");
            }
        }
       
        private void Button_Clicked_3(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_4(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_6(object sender, EventArgs e)
        {

        }

        private void MediaElement_MediaFailed(object sender, EventArgs e)
        {

        }

        private void CreateNew_Clicked(object sender, EventArgs e)
        {
            MenuItem menuItem = new MenuItem
            {
                Title = "Create Advert"
            };

            Navigation.PushAsync(new UploadPromotion(menuItem));
        }

        private void TopUp_Clicked(object sender, EventArgs e)
        {
            MenuItem menu = new MenuItem();
            menu.Title = "Service Balance Topup";

            Navigation.PushAsync(new TopupPromotion(menu));
        }

        private void ViewLeads_Clicked(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;
            //Navigation.PushAsync(new WebviewPage("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));

            Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/CustomerLeads/?CustomerId=" + uname + "&iDisplayStart=1&iDisplayLength=50", "My Leads", false, null));
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem menuItem = new YomoneyApp.MenuItem();
                var menu = JsonConvert.SerializeObject(view.CommandParameter);
                menuItem = JsonConvert.DeserializeObject<MenuItem>(menu);

                PromotionsViewModel promotion = new PromotionsViewModel(this, menuItem);
                promotion.EnableAd(menuItem);

                viewModel.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await DisplayAlert("Leads Error!", "Unable to gather leads from the server. Please check your internet connection and try again", "Ok");
            }
        }
    }
}