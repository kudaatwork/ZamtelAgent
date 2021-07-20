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
    public partial class QuoteDetails : ContentPage
    {

        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public QuoteDetails(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
            SelectedItem = selected;

            ButtonClose.Clicked += async (sender, e) =>
            {
                //await Navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
            };

            ButtonBack.Clicked += async (sender, e) =>
            {
                //await Navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetBids(SelectedItem);
        }
    }
}