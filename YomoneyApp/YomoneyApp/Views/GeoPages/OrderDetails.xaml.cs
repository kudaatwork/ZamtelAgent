using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Geo;

namespace YomoneyApp.Views.GeoPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetails : ContentPage
    {
        GeoLocationViewModel geoLocationViewModel;
        public OrderDetails()
        {
            InitializeComponent();
            BindingContext = geoLocationViewModel = new GeoLocationViewModel(this);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await geoLocationViewModel.ExecuteGetDemoOrderDetailsCommand();

           // orders.ItemsSource = geoLocationViewModel.OrderedItems;
        }

        private void btnSubmit_Clicked(object sender, EventArgs e)
        {

        }
    }
}