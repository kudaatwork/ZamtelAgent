using MvvmHelpers;
using Newtonsoft.Json;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models;
using YomoneyApp.Services;
using YomoneyApp.ViewModels;


namespace YomoneyApp.Views.GeoPages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Directions : ContentPage
    {
        MapPageViewModel mapPageViewModel;
        ChatViewModel chatViewModel;
        Location oldLocation = null;
        CancellationTokenSource cts;

        public Directions(string routeName, string role, decimal routeRate, decimal routeCost, string routeDuration, string routeDistance,
            string routeRealTimeDistance, string routeRealTimeInstructions)
        {
            InitializeComponent();
            BindingContext = mapPageViewModel = new MapPageViewModel(this);
            chatViewModel = new ChatViewModel(this);
            mapPageViewModel.RouteName = routeName;
            mapPageViewModel.Role = role;
            mapPageViewModel.RouteRate = routeRate;
            mapPageViewModel.RouteCost = routeCost;
            mapPageViewModel.RouteDuration = routeDuration;
            mapPageViewModel.RouteDistance = routeDistance;
            mapPageViewModel.RouteRealTimeDistance = routeRealTimeDistance;
            mapPageViewModel.RouteRealTimeInstructions = routeRealTimeInstructions;

            DisplayRoutes();           
        }

        #region LoadMap
        public async void DisplayRoutes()
        {
            var groupName = mapPageViewModel.RouteName.Replace(" ", "");

            chatViewModel.ExecuteJoinLocationPointGroupCommand(new RoutesInfo { Name = groupName });

            #region Using Points/Destinations
            try
            {
                var locations = MapPageViewModel.routes;

                if (locations != null)
                {
                    foreach (var item in locations)
                    {
                        Pin mapPins = new Pin()
                        {
                            Label = item.Name,
                            Type = PinType.Place,
                            // Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("CarPins.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "CarPins.png", WidthRequest = 20, HeightRequest = 20 }),
                            Position = new Position(Convert.ToDouble(item.Latitude), Convert.ToDouble(item.Longitude)),
                            Address = item.Address
                        };

                        map.Pins.Add(mapPins);
                    }
                }

                // Get Waypoints

                StringBuilder waypointsRoutes = new StringBuilder();

                if (locations.Count() > 2)
                {
                    locations.RemoveRange(0, 2);

                    for (int i = 0; i < locations.Count(); i++)
                    {
                        var waypoint = "|" + locations[i].Address;

                        waypointsRoutes.Append(waypoint);
                    }
                }

                map.Polylines.Clear();

                var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
                polyline.StrokeColor = Color.FromHex("#74b6ff");
                polyline.StrokeWidth = 8;

                // Draw Polylines for the first time

                var pathcontent = await mapPageViewModel.LoadRoutes("driving", waypointsRoutes.ToString());

                foreach (var p in pathcontent)
                {
                    polyline.Positions.Add(p);
                }

                map.Polylines.Add(polyline);

                var startLocation = new Position(Convert.ToDouble(locations[0].Latitude), Convert.ToDouble(locations[0].Longitude));

                map.MoveToRegion(MapSpan.FromCenterAndRadius(startLocation, Distance.FromMiles(0.3)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion                        
           
        }
        #endregion

        #region Start Tracing
        async void TrackPath_Clicked(System.Object sender, System.EventArgs e)
        {
            // Get Waypoints

           var locations = MapPageViewModel.routes;

           StringBuilder waypointsRoutes = new StringBuilder();

            if (locations.Count() > 2)
            {
                locations.RemoveRange(0, 2);

                for (int i = 0; i < locations.Count(); i++)
                {
                    var waypoint = "|" + locations[i].Address;

                    waypointsRoutes.Append(waypoint);
                }              
            }

            var pathcontent = await mapPageViewModel.LoadRoutes("driving", waypointsRoutes.ToString());

            map.Polylines.Clear();

            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.FromHex("#74b6ff");
            polyline.StrokeWidth = 8;

            foreach (var pLinePosition in pathcontent)
            {
                polyline.Positions.Add(pLinePosition);
            }

            map.Polylines.Add(polyline);

            var polyline2 = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline2.StrokeColor = Color.FromHex("#74b6ff");
            polyline2.StrokeWidth = 8;

            foreach (var polyLinePosition in pathcontent)
            {
                polyline2.Positions.Add(polyLinePosition);
            }

            map.Polylines.Add(polyline2);

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));

                cts = new CancellationTokenSource();

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                // var location = MapPageViewModel.routes[0];

                if (location != null)
                {
                    var currentPosition = new Position(Convert.ToDouble(location.Latitude), Convert.ToDouble(location.Longitude));

                    map.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Distance.FromMiles(0.3)));

                    #region Live Code
                    Device.StartTimer(TimeSpan.FromSeconds(10), () =>
                    {
                        UpdateLiveLocations();
                        return true;
                    });
                    #endregion

                    #region Demo/Simulation Code
                    //var positionIndex = 1;

                    //Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    //{
                    //    if (pathcontent.Count > positionIndex)
                    //    {
                    //        UpdatePostions(pathcontent[positionIndex]);
                    //        positionIndex++;
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;
                    //    }
                    //});
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Update Simulated Locations
        async void UpdatePostions(Xamarin.Forms.GoogleMaps.Position position)
        {
            if (map.Pins.Count == 1 && map.Polylines != null && map.Polylines?.Count > 1)
                return;

            var destinationPin = map.Pins.FirstOrDefault();

            if (destinationPin != null)
            {
                destinationPin.Position = new Xamarin.Forms.GoogleMaps.Position(position.Latitude, position.Longitude);

                var groupName = mapPageViewModel.RouteName.Replace(" ", "");

                chatViewModel.ExecuteSendLocationPointCommand(new RoutesInfo { Latitude = position.Latitude.ToString(), Longitude = position.Longitude.ToString(), Name = groupName });

                map.MoveToRegion(MapSpan.FromCenterAndRadius(destinationPin.Position, Distance.FromMiles(0.3)));

                var placemarks = await Geocoding.GetPlacemarksAsync(destinationPin.Position.Latitude, destinationPin.Position.Longitude);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =
                        $"AdminArea:       {placemark.AdminArea}\n" +
                        $"CountryCode:     {placemark.CountryCode}\n" +
                        $"CountryName:     {placemark.CountryName}\n" +
                        $"FeatureName:     {placemark.FeatureName}\n" +
                        $"Locality:        {placemark.Locality}\n" +
                        $"PostalCode:      {placemark.PostalCode}\n" +
                        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                        $"SubLocality:     {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Location :       {placemark.Location}\n" +
                        $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    Debug.WriteLine(geocodeAddress);
                }

                CrossLocalNotifications.Current.Show("Location Updated", "You checked in to " + placemark.FeatureName + " " + placemark.Locality + " " + placemark.SubLocality, 101, DateTime.Now.AddSeconds(5));

                if (MapPageViewModel.googleDirectionGlobal.Routes != null && MapPageViewModel.googleDirectionGlobal.Routes.Count > 0)
                {
                    var legs = MapPageViewModel.googleDirectionGlobal.Routes.First().Legs;

                    //var leg = legs.Where(x => x.Steps.Where(x => (PolylineHelper.Decode(x.Polyline.Points).Where(x => x.Latitude == position.Latitude && x.Longitude == position.Longitude)))).FirstOrDefault();

                    foreach (var leg in legs)
                    {
                        foreach (var step in leg.Steps)
                        {
                            var stepPositions = PolylineHelper.Decode(step.Polyline.Points);

                            if (stepPositions.FirstOrDefault().Latitude == position.Latitude && stepPositions.FirstOrDefault().Longitude == position.Longitude)
                            {
                                mapPageViewModel.RouteRealTimeDistance = step.Distance.Text;
                                mapPageViewModel.RouteRealTimeDuration = step.Duration.Text;
                                mapPageViewModel.RouteRealTimeInstructions = StripHTML(step.HtmlInstructions);

                                static string StripHTML(string input)
                                {
                                    return Regex.Replace(input, "<.*?>", String.Empty);
                                }

                                break;
                            }
                        }
                    }
                }

                var previousPosition = map.Polylines?.FirstOrDefault()?.Positions?.FirstOrDefault();
                map.Polylines?.FirstOrDefault()?.Positions?.Remove(previousPosition.Value);
            }
            else
            {
                map.Polylines?.FirstOrDefault()?.Positions?.Clear();
            }
        }
        #endregion       

        #region Update Live Locations
        async Task UpdateLiveLocations()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));

                cts = new CancellationTokenSource();

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    var newPosition = new Position(location.Latitude, location.Longitude);

                    var groupName = mapPageViewModel.RouteName.Replace(" ", "");

                    chatViewModel.ExecuteSendLocationPointCommand(new RoutesInfo { Latitude = newPosition.Latitude.ToString(), Longitude = currentPosition.Longitude.ToString(), Name = groupName });

                    if (oldLocation == null)
                    {
                        oldLocation = location;

                        map.MoveToRegion(MapSpan.FromCenterAndRadius(newPosition, Distance.FromMiles(0.3)));
                    }

                    if (location.Latitude != oldLocation.Latitude || location.Longitude != oldLocation.Longitude)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(newPosition, Distance.FromMiles(0.3)));
                                                
                        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                        
                        var placemark = placemarks?.FirstOrDefault();

                        if (placemark != null)
                        {
                            var geocodeAddress =
                                $"AdminArea:       {placemark.AdminArea}\n" +
                                $"CountryCode:     {placemark.CountryCode}\n" +
                                $"CountryName:     {placemark.CountryName}\n" +
                                $"FeatureName:     {placemark.FeatureName}\n" +
                                $"Locality:        {placemark.Locality}\n" +
                                $"PostalCode:      {placemark.PostalCode}\n" +
                                $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                                $"SubLocality:     {placemark.SubLocality}\n" +
                                $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                                $"Location :       {placemark.Location}\n" +
                                $"Thoroughfare:    {placemark.Thoroughfare}\n";

                            Debug.WriteLine(geocodeAddress);
                        }

                        CrossLocalNotifications.Current.Show("Location Updated", "You checked in to " + placemark.FeatureName + " " + placemark.Locality + " " + placemark.SubLocality, 101, DateTime.Now.AddSeconds(5));

                        if (MapPageViewModel.googleDirectionGlobal.Routes != null && MapPageViewModel.googleDirectionGlobal.Routes.Count > 0)
                        {
                            var legs = MapPageViewModel.googleDirectionGlobal.Routes.First().Legs;

                            //var leg = legs.Where(x => x.Steps.Where(x => (PolylineHelper.Decode(x.Polyline.Points).Where(x => x.Latitude == position.Latitude && x.Longitude == position.Longitude)))).FirstOrDefault();

                            foreach (var leg in legs)
                            {
                                foreach (var step in leg.Steps)
                                {
                                    //  var stepPositions = PolylineHelper.Decode(step.Polyline.Points);

                                    // if (stepPositions.FirstOrDefault().Latitude == position.Latitude && stepPositions.FirstOrDefault().Longitude == position.Longitude)
                                    // {
                                    mapPageViewModel.RouteRealTimeDistance = step.Distance.Text;
                                    mapPageViewModel.RouteRealTimeDuration = step.Duration.Text;
                                    mapPageViewModel.RouteRealTimeInstructions = StripHTML(step.HtmlInstructions);

                                    static string StripHTML(string input)
                                    {
                                        return Regex.Replace(input, "<.*?>", String.Empty);
                                    }

                                    break;
                                    // }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Customize Behaviour when map becomes visible
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await UpdateLiveLocations();
        }
        #endregion

        #region Clear Map Elements and Back to Home
        private void Button_Clicked(object sender, EventArgs e)
        {
            map.Polylines.Clear();
            map.Pins.Clear();

            Navigation.PushAsync(new HomePage());
        }
        #endregion

        #region Open Route Details
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }

            Navigation.PushAsync(new RouteDetails(mapPageViewModel.RouteName, mapPageViewModel.Role, mapPageViewModel.RouteRate, mapPageViewModel.RouteCost, mapPageViewModel.RouteDuration,
                                mapPageViewModel.RouteDistance));
        }
        #endregion
    }
}