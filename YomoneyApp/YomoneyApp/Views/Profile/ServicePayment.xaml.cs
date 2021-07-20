using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spend
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicePayment : ContentPage
    {
        WalletServicesViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public ServicePayment(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new WalletServicesViewModel(this);
            SelectedItem = selected;
            PickerStore.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
            };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if ((viewModel.Category != null && viewModel.Category != "") || viewModel.IsBusy)
            {

            }
            else
            {
                try
                {
                    var stores = await viewModel.GetPaymentsAsync();
                    PickerStore.Items.Clear();
                    foreach (var store in stores)
                        PickerStore.Items.Add(store.Title.Trim());
                }
                catch (Exception ex)
                {

                }
                try
                {
                    var stores = await viewModel.GetServicesAsync(SelectedItem);
                    PickerStore.Items.Clear();
                    foreach (var store in stores)
                    PickerStore.Items.Add(store.Title.Trim());
                }
                catch (Exception ex)
                {
                }

            }
        }
    }
}