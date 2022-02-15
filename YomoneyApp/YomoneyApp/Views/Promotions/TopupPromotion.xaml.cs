using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Promotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopupPromotion : ContentPage
    {
        PromotionsViewModel promotionsViewModel;
        RequestViewModel viewModel;

        public TopupPromotion(MenuItem menuItem)
        {
            InitializeComponent();
            BindingContext = promotionsViewModel = new PromotionsViewModel(this, menuItem);
            viewModel = new RequestViewModel(this);

            Currency.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.Currency = Currency.Items[Currency.SelectedIndex];
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (string.IsNullOrEmpty(promotionsViewModel.Currency))
                {
                    promotionsViewModel.IsBusy= true;
                    promotionsViewModel.Message = "Loading Currencies...";

                    var currencies = await viewModel.GetCurrenciesAsync();
                    Currency.Items.Clear();
                    foreach (var cur in currencies)
                        Currency.Items.Add(cur.Title.Trim());
                    
                    promotionsViewModel.IsBusy = false;
                }

                //MenuItem selected = new MenuItem();
                //selected.Title = "Airtime";
                //selected.Image = "airtime_icon.png";
                //selected.ServiceId = 7;
                //selected.SupplierId = "All";
                //selected.Section = "Yomoney";
                //selected.TransactionType = 2;

                //if ((viewModel.Category != null && viewModel.Category != "") || viewModel.IsBusy)
                //{

                //}
                //else
                //{
                //    var stores = await promotionsViewModel.GetYoAppServicesAsync("Pinless", selected);
                //    PickerStore.Items.Clear();
                //    foreach (var store in stores)
                //        PickerStore.Items.Add(store.Title.Trim());
                //}                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Loading Error!", "Unable to gather YoApp Services", "Ok");
            }
            
        }
    }
}