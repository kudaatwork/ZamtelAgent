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
    public partial class OrderPackages : ContentPage
    {
        GeoLocationViewModel geoLocationViewModel;
        MenuItem SelectedPackage;
        public OrderPackages()
        {
            InitializeComponent();
            BindingContext = geoLocationViewModel = new GeoLocationViewModel(this);
            SelectedPackage = new MenuItem();
        }

        private void packages_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectedPackage = e.SelectedItem as MenuItem;
            
            geoLocationViewModel.ExecuteGetPackageLinesSelectedCommand(SelectedPackage);

            Navigation.PushAsync(new OrderDetails());
        }
    }
}