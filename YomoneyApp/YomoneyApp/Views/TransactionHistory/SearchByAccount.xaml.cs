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

            PickerCategory.SelectedIndexChanged += async (sender, e) =>
            {
                viewModel.SuperCategory = PickerCategory.Items[PickerCategory.SelectedIndex];

                if (viewModel.SuperCategory == "PURCHASES")
                {
                    SelectedItem.TransactionType = 2;
                }
                else
                {
                    SelectedItem.TransactionType = 3;
                }

                try
                {                   
                    var stores = await viewModel.GetHistoryPickerAsync(SelectedItem.TransactionType.ToString());
                    PickerStore.Items.Clear();
                    foreach (var store in stores)
                        PickerStore.Items.Add(store.Title.Trim());                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await DisplayAlert("Error!", "Unable to gather billers because of a server error. Contact customer support", "OK");
                }               
            };

            PickerStore.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
            };
        }
        protected override async void OnAppearing()
        {
            List<string> serviceCategories = new List<string>();

            serviceCategories.Add("PURCHASES");
            serviceCategories.Add("PAYMENTS");

            base.OnAppearing();
            if ((viewModel.Category != null && viewModel.Category != "") || viewModel.IsBusy)
            {

            }
            else
            {
                //try
                //{
                //    var stores = await viewModel.GetHistoryPickerAsync(SelectedItem.TransactionType.ToString());
                //    PickerStore.Items.Clear();
                //    foreach (var store in stores)
                //        PickerStore.Items.Add(store.Title.Trim());
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    await DisplayAlert("Error!", "Unable to gather billers because of a server error. Contact customer support", "OK");
                //}

                try
                {
                    foreach (var serviceCategory in serviceCategories)
                       PickerCategory.Items.Add(serviceCategory.Trim());                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await DisplayAlert("Error!", "Unable to gather billers because of a server error. Contact customer support", "OK");
                }

            }
        }
    }
}