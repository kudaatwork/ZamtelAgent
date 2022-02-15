using YomoneyApp.Services;
using YomoneyApp.Views.Webview;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {   
        WalletServicesViewModel viewModel;
        MenuItem SelectedItem;
        CountryPickerViewModel countryPickerViewModel;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public PaymentPage(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.WalletServicesViewModel(this);
            countryPickerViewModel = new CountryPickerViewModel(this);

            if (string.IsNullOrEmpty(mnu.Currency))
            {
                mnu.Currency = "ZWL";
            }

            viewModel.Currency = mnu.Currency;
            viewModel.Budget = string.Format("{0:#.00}", mnu.Amount);
            viewModel.Title = mnu.Title;

            SelectedItem = mnu;

            viewModel.ServiceId = mnu.ServiceId;

            PickerStore.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];

                if (viewModel.Category != "PAYNOW PAYMENT")
                {
                    viewModel.Ptitle = viewModel.Category.Substring(0, 1) + viewModel.Category.Substring(1, viewModel.Category.Length - 1).ToLower() + " Account Number ";
                    viewModel.ShowNavigation = true;
                }
                else
                {
                    if (viewModel.Category != "WEB PAYMENT")
                    {
                        viewModel.Ptitle = viewModel.Category.Substring(0, 1) + viewModel.Category.Substring(1, viewModel.Category.Length - 1).ToLower() + " Account Number ";
                        viewModel.ShowNavigation = true;
                    }
                    else
                    {
                        viewModel.ShowNavigation = false;
                    }
                }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                var stores = await viewModel.GetPaymentsAsync(SelectedItem);
                
                if (viewModel.Category == null || viewModel.Category == "")
                {
                    PickerStore.Items.Clear();
                    foreach (var store in stores)
                        PickerStore.Items.Add(store.Title.Trim());
                }

                viewModel.ExecuteGetCurrentGeolocationCommand();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Payment Error!", "Unable to gather the payment options", "Ok");
            }

        }
    }
}