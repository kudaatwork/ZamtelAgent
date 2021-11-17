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

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuoteDetails : ContentPage
    {

        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public QuoteDetails(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
            SelectedItem = selected;

            ButtonClose.Clicked += async (sender, e) =>
            {
                //await Navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
            };

            ButtonBack.Clicked += async (sender, e) =>
            {
                //await Navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetBids(SelectedItem);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var view = sender as Xamarin.Forms.Button;
            MenuItem mn = new YomoneyApp.MenuItem();
            var x = JsonConvert.SerializeObject(view.CommandParameter);
            mn = JsonConvert.DeserializeObject<MenuItem>(x);

            if (!string.IsNullOrEmpty(mn.SupplierId))
            {                 
                char[] delimite = new char[] { '_' };

                string[] parts = mn.SupplierId.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                if (parts != null) 
                {
                    var supplierId = parts[0];
                   
                    await Navigation.PushModalAsync(new WebviewPage("http://192.168.100.150:5000/Mobile/JobProfile?id=" + supplierId, "Job Profile", true, null));
                }
                else
                {
                    await DisplayAlert("Error!", "User does not have enough information so, you cannot see their profile", "Ok");
                }
            }
            else
            {
                await DisplayAlert ("Error!", "User does not have enough information so, you cannot see their profile", "Ok");
            }
        }
    }
}