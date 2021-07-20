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
    public partial class TockenRecharge : ContentPage
    {
        SpendViewModel viewModel;
        MenuItem selected;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public TockenRecharge(MenuItem mm)
        {
            InitializeComponent();
            selected = mm;
            BindingContext = viewModel = new SpendViewModel(this);

            PickerStore.SelectedIndexChanged += async (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
                var stores = await viewModel.GetProductsAsync();
                foreach (var store in stores)
                    PickerAmount.Items.Add(store.Amount.Trim());
            };

            PickerAmount.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Budget = PickerAmount.Items[PickerAmount.SelectedIndex];
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
                    var stores = await viewModel.GetTopupAsync("Token", selected);
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