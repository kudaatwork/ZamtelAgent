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
    public partial class Recharge : ContentPage
    {
        SpendViewModel viewModel;
        MenuItem selected;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public Recharge(MenuItem mm)
        {

            InitializeComponent();
            selected = mm;
            BindingContext = viewModel = new SpendViewModel(this);
            viewModel.RetryText = "Retry";
            if (mm.Note == "Reward Service")
            {
                viewModel.Budget = mm.Amount;
                viewModel.Retry = true;
                viewModel.IsConfirm = false;
                viewModel.RetryText = "Get Reward";
            }
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
                    var stores = await viewModel.GetTopupAsync("Pinless", selected);
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