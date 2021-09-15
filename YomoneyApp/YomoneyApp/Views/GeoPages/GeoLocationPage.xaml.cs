using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading;
using YomoneyApp.ViewModels.Geo;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using YomoneyApp.Models.PlacesModel;
using System.Net.Http;
using Xamarin.Forms.GoogleMaps;

namespace YomoneyApp.Views.GeoPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoLocationPage : ContentPage
    {
        GeoLocationViewModel geoLocationViewModel;

        CancellationTokenSource cts;

        private HttpClient client;

        private Pin routePin1 = new Pin
        {
            Label = "My Current Location"
        };

        private Pin routePin2 = new Pin
        {
            Label = "Samy Levy Village",            
        };

        public GeoLocationPage(Position position)
        {
            InitializeComponent();
            //BindingContext = geoLocationViewModel = new GeoLocationViewModel(this);
            //geoLocationViewModel.Position = position;
            //Task.Delay(2000);
            //UpdateMap();

            client = new HttpClient();

            GetCurrentLocation();
        }

        #region Testing Region
        List<Place> placesList = new List<Place>();

        private async void UpdateMap()
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(GeoLocationPage)).Assembly;

                Stream stream = assembly.GetManifestResourceStream("YomoneyApp.Places.json");

                string text = string.Empty;

                using (var reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                var resultObject = JsonConvert.DeserializeObject<Places>(text);

                foreach (var place in resultObject.results)
                {
                    placesList.Add(new Place
                    {
                        PlaceName = place.name,
                        Address = place.vicinity,
                        Location = place.geometry.location,
                        Position = new Position(place.geometry.location.lat, place.geometry.location.lng),
                        //Icon = place.icon,
                        //Distance = $"{GetDistance(lat1, lon1, place.geometry.location.lat, place.geometry.location.lng, DistanceUnit.Kiliometers).ToString("N2")}km",
                        //OpenNow = GetOpenHours(place?.opening_hours?.open_now)
                    });
                }

                MyMap.ItemsSource = placesList;
                //PlacesListView.ItemsSource = placesList;
                //var loc = await Xamarin.Essentials.Geolocation.GetLocationAsync();
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(47.6370891183, -122.123736172), Distance.FromKilometers(100)));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion

        //private async void btnLastKnownLocation_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var location = await Geolocation.GetLastKnownLocationAsync();

        //        if (location != null)
        //        {
        //            location = await Geolocation.GetLocationAsync(new GeolocationRequest
        //            {
        //                DesiredAccuracy = GeolocationAccuracy.Medium,
        //                Timeout = TimeSpan.FromSeconds(30)
        //            }); ;
        //        }

        //        if (location == null)
        //        {
        //            await DisplayAlert("Geolocation Error!", "No GPS", "OK");
        //        }
        //        else
        //        {
        //            await DisplayAlert("Geolocation Co-ordinates", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "OK");
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Handle not supported on device exception
        //        await DisplayAlert("Geolocation Error!", "Device not supported", "OK");
        //    }
        //    catch (FeatureNotEnabledException fneEx)
        //    {
        //        // Handle not enabled on device exception
        //        await DisplayAlert("Geolocation Error!", "Device not supported", "OK");
        //    }
        //    catch (PermissionException pEx)
        //    {
        //        // Handle permission exception
        //        await DisplayAlert("Geolocation Error!", "Provide the necessary permission", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to get location
        //        await DisplayAlert("Geolocation Error!", "Unable to get location", "OK");
        //    }
        //}

       
        private async void btnCurrentLocation_Clicked(object sender, EventArgs e)
        {
            // UpdateMap();

           await GetCurrentLocation();
        }

        async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));

                cts = new CancellationTokenSource();

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    MyMap.Pins.Add(routePin1);

                    MyMap.Pins.Add(routePin2);

                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        routePin1.Position = new Position(location.Latitude, location.Longitude);

                        routePin2.Position = new Position(-18.3308368, 29.4508492);

                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(routePin1.Position, Distance.FromKilometers(1)));

                       // MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(routePin2.Position, Distance.FromKilometers(1)));

                        return true;
                    });                   

                    //await DisplayAlert("Geolocation Co-ordinates", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Geolocation Error!", "Device not supported", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Geolocation Error!", "Device not supported", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Geolocation Error!", "Provide the necessary permissions", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Geolocation Error!", "Unable to get location", "OK");
            }
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        



       
    }
}
