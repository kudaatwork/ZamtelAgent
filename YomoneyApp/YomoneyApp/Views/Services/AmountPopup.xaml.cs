using Microsoft.EntityFrameworkCore.Query;
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
    public partial class AmountPopup : ContentPage
    {
        WalletServicesViewModel viewModel;
        MenuItem SelectedItem;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public AmountPopup(MenuItem mn)
        {
            InitializeComponent();
            viewModel = new YomoneyApp.WalletServicesViewModel(this);
            viewModel.Balance = mn.Title;
            viewModel.Section = mn.Description;
            SelectedItem = mn;
            BindingContext = viewModel;
            PickerCurrency.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Currency = PickerCurrency.Items[PickerCurrency.SelectedIndex];
            };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (string.IsNullOrEmpty(SelectedItem.Currency))
                {
                    SelectedItem.Currency = "ZWL";
                }
                var stores = await viewModel.GetCurrenciesAsync(SelectedItem);
                PickerCurrency.Items.Clear();
                

                foreach (var store in stores)
                {

                    PickerCurrency.Items.Add(store.Title.Trim());
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}