using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyVoucher : ContentPage
    {
        ServiceViewModel viewModel;
        WalletServicesViewModel walletViewModel ;
        MenuItem SelectedItem;
        CountryPickerViewModel countryPickerViewModel;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public BuyVoucher(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, selected);
            walletViewModel = new WalletServicesViewModel(this);
            countryPickerViewModel = new CountryPickerViewModel(this);

            SelectedItem = selected;
            PickerCurrency.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Currency = PickerCurrency.Items[PickerCurrency.SelectedIndex];
            };
        }
        private async void Voucher_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                await viewModel.ExecuteGetVoucherCommand(SelectedItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Error!", "Sorry, there has been an error in submitting your request. Please, try again", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {                
                var stores = await walletViewModel.GetCurrenciesAsync(SelectedItem);

                PickerCurrency.Items.Clear();

                if (string.IsNullOrEmpty(SelectedItem.Currency))
                {
                    SelectedItem.Currency = "ZWL";
                }

                foreach (var store in stores)
                {
                    PickerCurrency.Items.Add(store.Title.Trim());
                }

                viewModel.ExecuteGetCurrentGeolocationCommand();
            }
            catch (Exception ex)
            {

            }
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetSeledtedProvider(SelectedItem);

        }
    }
}