using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.TransactionHistory
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchByAccount : ContentPage
    {
        TransactionViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public SearchByAccount()
        {
            MenuItem selected = new MenuItem { Title = "Transaction History", Description = "View your spending history", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 3 };
            InitializeComponent();
            BindingContext = viewModel = new TransactionViewModel(this, selected);
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
                    var stores = await viewModel.GetHistoryPickerAsync(SelectedItem.TransactionType.ToString());
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