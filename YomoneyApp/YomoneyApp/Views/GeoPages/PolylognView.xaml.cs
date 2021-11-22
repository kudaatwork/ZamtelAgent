using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.GeoPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PolylognView : ContentPage
    {
        HomeViewModel viewModel;
        public PolylognView()
        {
            InitializeComponent();
            BindingContext = viewModel = new HomeViewModel(this);

            DisplayPolygon();
        }

        #region LoadPolygon
        public async void DisplayPolygon()
        {
            try
            {
                var locations = HomeViewModel.polygonLocationPoints; // Get Locations From the Server

                map.Polygons.Clear();
                var polygon = new Xamarin.Forms.GoogleMaps.Polygon();
                polygon.StrokeColor = Color.FromHex("#74b6ff");
                polygon.FillColor = Color.FromHex("#c7e2ff");
                polygon.StrokeWidth = 3;

                // Draw Polygon on the Map

                foreach (var p in locations)
                {
                    polygon.Positions.Add(p);
                }
                               
                map.Polygons.Add(polygon);

                var startLocation = new Position(Convert.ToDouble(locations[0].Latitude), Convert.ToDouble(locations[0].Longitude));

                map.MoveToRegion(MapSpan.FromCenterAndRadius(startLocation, Distance.FromMiles(0.3)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.InnerException + ex.StackTrace);

                await DisplayAlert("Error!", "There has been an error in loading your Polygon", "Ok");
            }
        }
        #endregion
    }
}