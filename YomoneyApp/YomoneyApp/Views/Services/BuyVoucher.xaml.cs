using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyVoucher : ContentPage
    {
        ServiceViewModel viewModel;
        WalletServicesViewModel walletViewModel ;
        MenuItem SelectedItem;

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

            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                
                var stores = await walletViewModel.GetCurrenciesAsync();
                PickerCurrency.Items.Clear();
                if (string.IsNullOrEmpty(SelectedItem.Currency))
                {
                    SelectedItem.Currency = "USD";
                }

                foreach (var store in stores)
                {

                    PickerCurrency.Items.Add(store.Title.Trim());
                }
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