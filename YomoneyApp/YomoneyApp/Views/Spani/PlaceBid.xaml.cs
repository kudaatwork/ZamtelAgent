using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceBid : ContentPage
    {
        RequestViewModel viewModel;
        public PlaceBid(MenuItem itm)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
            viewModel.JobPostId = itm.Id;
            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };
            PickerCurrency.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Currency = PickerCurrency.Items[PickerCurrency.SelectedIndex];
            };
        }
        protected override async void OnAppearing()
        {
           
            try
            {
              
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