using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spani
{
    
    public partial class JobRequest : ContentPage
    {
        RequestViewModel viewModel;

        public JobRequest()
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);

            PickerStore.SelectedIndexChanged += async (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
                PickerSubcategory.Items.Clear();
                var stores = await viewModel.GetStoreAsync("JobSubCategory");
                foreach (var store in stores)
                    PickerSubcategory.Items.Add(store.Title.Trim());
            };

            PickerCurrency.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Currency = PickerCurrency.Items[PickerCurrency.SelectedIndex];
            };

            PickerSubcategory.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Subcategory = PickerSubcategory.Items[PickerSubcategory.SelectedIndex];
            };

            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Categories.Count > 0 || viewModel.IsBusy)
            {
                PickerStore.Items.Clear();
                foreach (var store in viewModel.Categories)
                    PickerStore.Items.Add(store.Title.Trim());
                return;
            }
            try
            {
                var stores = await viewModel.GetStoreAsync("JobSectors");
                PickerStore.Items.Clear();
                foreach (var store in stores)
                    PickerStore.Items.Add(store.Title.Trim());

                //var currencies = await viewModel.GetCurrenciesAsync();
                //PickerCurrency.Items.Clear();
                //foreach (var cur in currencies)
                //    PickerCurrency.Items.Add(cur.Title.Trim());
            }
            catch (Exception ex)
            {
               
            }

            try
            {
                //var stores = await viewModel.GetStoreAsync("JobSectors");
                //PickerStore.Items.Clear();
                //foreach (var store in stores)
                //    PickerStore.Items.Add(store.Title.Trim());

                var currencies = await viewModel.GetCurrenciesAsync();
                PickerCurrency.Items.Clear();
                foreach (var cur in currencies)
                    PickerCurrency.Items.Add(cur.Title.Trim());
            }
            catch (Exception ex)
            {

            }

        }
    }
}