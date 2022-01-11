using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopupPage : ContentPage
    {
        WalletServicesViewModel viewModel;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public TopupPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.WalletServicesViewModel(this);

            PickerStore.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
                if (viewModel.Category != "PAYNOW PAYMENT")
                {
                    viewModel.Ptitle = viewModel.Category.Substring(0, 1) + viewModel.Category.Substring(1, viewModel.Category.Length - 1).ToLower() + " Account Number ";
                    viewModel.ShowNavigation = true;
                }
            };
        }

   
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                var mnu = new MenuItem();
                var stores = await viewModel.GetPaymentsAsync(mnu);
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