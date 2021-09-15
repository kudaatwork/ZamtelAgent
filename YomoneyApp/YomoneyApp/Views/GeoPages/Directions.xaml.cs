using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels;

namespace YomoneyApp.Views.GeoPages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Directions : ContentPage
    {
        MapPageViewModel mapPageViewModel;

        CancellationTokenSource cts;

        public Directions()
        {
            InitializeComponent();
            BindingContext = mapPageViewModel = new MapPageViewModel(this);            
        }

        #region Map Theme
        private void ApplyMapTheme()
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"YomoneyApp.MapResources.MapTheme.json");
            string themeFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                themeFile = reader.ReadToEnd();
                map.MapStyle = MapStyle.FromJson(themeFile);
            }
        }
        #endregion

        #region DestinationLocations
        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            var locations = await mapPageViewModel.LoadDestinationLocations();

            if (locations != null)
            {
                foreach (var item in locations)
                {
                    Pin VehiclePins = new Pin()
                    {
                        Label = "Destination Location",
                        Type = PinType.Place,
                        // Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("CarPins.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "CarPins.png", WidthRequest = 20, HeightRequest = 20 }),
                        Position = new Position(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude)),
                    };

                    map.Pins.Add(VehiclePins);
                }
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));

            cts = new CancellationTokenSource();

            var location = await Geolocation.GetLocationAsync(request, cts.Token);

            //This is your location and it should be near to your car location.
            var positions = new Position(location.Latitude, location.Longitude);//Latitude, Longitude
            map.MoveToRegion(MapSpan.FromCenterAndRadius(positions, Distance.FromMeters(500)));

        }
        #endregion        

        #region Update Vehicles
        async void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new WaletServices());
        }

        private bool TimerStarted()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Compass.Start(SensorSpeed.UI, applyLowPassFilter: true);
                Compass.ReadingChanged += Compass_ReadingChanged;
                map.Pins.Clear();
                map.Polylines.Clear();
                //Get the cars nearrby from api but here we are hardcoding the contents
                var contents = await mapPageViewModel.LoadDestinationLocations();
                if (contents != null)
                {
                    foreach (var item in contents)
                    {
                        Pin VehiclePins = new Pin()
                        {
                            Label = "Cars",
                            Type = PinType.Place,
                            Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("CarPins.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "CarPins.png", WidthRequest = 30, HeightRequest = 30 }),
                            Position = new Position(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude)),
                            Rotation = ToRotationPoints(headernothvalue)
                        };
                        map.Pins.Add(VehiclePins);
                    }
                }
            }

            );
            Compass.Stop();
            return true;
        }

        private float ToRotationPoints(double headernothvalue)
        {
            return (float)headernothvalue;

        }

        double headernothvalue;
        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            headernothvalue = data.HeadingMagneticNorth;
        }
        #endregion

        void map_PinDragStart(System.Object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {


        }

        async void map_PinDragEnd(System.Object sender, Xamarin.Forms.GoogleMaps.PinDragEventArgs e)
        {
            var positions = new Position(e.Pin.Position.Latitude, e.Pin.Position.Longitude);//Latitude, Longitude
            map.MoveToRegion(MapSpan.FromCenterAndRadius(positions, Distance.FromMeters(500)));
            await App.Current.MainPage.DisplayAlert("Alert", "Pick up location : Latitude :" + e.Pin.Position.Latitude + " Longitude :" + e.Pin.Position.Longitude, "Ok");
        }

        //async void PickupButton_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    await GetCurrentLocation();
        //}

        async void TrackPath_Clicked(System.Object sender, System.EventArgs e)
        {
            var pathcontent = await mapPageViewModel.LoadRoute();
            
            map.Polylines.Clear();

            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.LightBlue;
            polyline.StrokeWidth = 7;

            foreach (var p in pathcontent)
            {
                polyline.Positions.Add(p);
            }

            map.Polylines.Add(polyline);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(polyline.Positions[0].Latitude, polyline.Positions[0].Longitude ), Xamarin.Forms.GoogleMaps.Distance.FromKilometers(2.5)));

            foreach (var item in mapPageViewModel.yoAppLocations)
            {
                var pin = new Xamarin.Forms.GoogleMaps.Pin
                {
                    Type = PinType.Place,
                    Position = new Xamarin.Forms.GoogleMaps.Position(item.Latitude, item.Longitude),
                    Label = "Destination Pin",
                };

                map.Pins.Add(pin);
            }

            var positionIndex = 1;

            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                if (pathcontent.Count > positionIndex)
                {
                    UpdatePostions(pathcontent[positionIndex]);
                    positionIndex++;
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        async void UpdatePostions(Xamarin.Forms.GoogleMaps.Position position)
        {
            if (map.Pins.Count == 1 && map.Polylines != null && map.Polylines?.Count > 1)
                return;

            var destinationPin = map.Pins.FirstOrDefault();

            if (destinationPin != null)
            {
                destinationPin.Position = new Xamarin.Forms.GoogleMaps.Position(position.Latitude, position.Longitude);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(destinationPin.Position, Distance.FromMeters(200)));
                var previousPosition = map.Polylines?.FirstOrDefault()?.Positions?.FirstOrDefault();
                map.Polylines?.FirstOrDefault()?.Positions?.Remove(previousPosition.Value);
            }
            else
            {
                map.Polylines?.FirstOrDefault()?.Positions?.Clear();
            }
        }

        #region MyRegion
        //async Task GetCurrentLocation()
        //{
        //    try
        //    {
        //        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));

        //        cts = new CancellationTokenSource();

        //        var location = await Geolocation.GetLocationAsync(request, cts.Token);

        //        if (location != null)
        //        {
        //            Pin routePin1 = new Pin
        //            {
        //                Label = "My Current Location",
        //                Type = PinType.Place,
        //                IsDraggable = true
        //            };

        //            map.Pins.Add(routePin1);                    

        //            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        //            {
        //                routePin1.Position = new Position(location.Latitude, location.Longitude);                       

        //                map.MoveToRegion(MapSpan.FromCenterAndRadius(routePin1.Position, Distance.FromKilometers(100)));

        //                return true;
        //            });
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
        //        await DisplayAlert("Geolocation Error!", "Provide the necessary permissions", "OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Unable to get location
        //        await DisplayAlert("Geolocation Error!", "Unable to get location", "OK");
        //    }
        //}

        //protected override void OnDisappearing()
        //{
        //    if (cts != null && !cts.IsCancellationRequested)
        //        cts.Cancel();
        //    base.OnDisappearing();
        //}
        #endregion
    }
}