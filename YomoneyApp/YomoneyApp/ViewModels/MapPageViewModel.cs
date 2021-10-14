using MvvmHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using YomoneyApp.Models;
using YomoneyApp.Services;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        #region ViewModel Properties & Variables
      
        public string HostDomain = "https://www.yomoneyservice.com";

        AccountViewModel accountViewModel = new AccountViewModel(null);
     
        #endregion

        public MapPageViewModel(Page page) : base(page)
        {
        }

        public static GoogleDirection googleDirectionGlobal;

        public static List<RoutesInfo> routes = new List<RoutesInfo>();

        static string originAddress = string.Empty;
        static string destinationAddress = string.Empty;

        #region Display Map with points from Server
        public async void DisplayMap(string serverData)
        {
            try
            {
                string dencodedServerData = HttpUtility.HtmlDecode(serverData);

                char[] delimite = new char[] { '_' };

                string[] parts = dencodedServerData.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 5)
                {
                    accountViewModel.CheckData(serverData);
                }
                else
                {
                    var routeId = parts[0];
                    var name = parts[1];
                    var rate = Convert.ToDecimal(parts[2]);
                    var role = parts[3].Trim().ToLower();
                    var destinations = parts[4];

                    RouteName = parts[1].Trim();
                    RouteRate = Convert.ToDecimal(parts[2]);
                    Role = parts[3].Trim();

                    routes = JsonConvert.DeserializeObject<List<RoutesInfo>>(destinations);
                    originAddress = routes[0].Address;
                    destinationAddress = routes[1].Address;

                    if (rate == 0) // Only if Rate is Greater than 0
                    {
                        if (role == "driver" || role == "passenger") // Drivers and Passengers
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new Directions(RouteName, Role, RouteRate, RouteCost, RouteDuration,
                                    RouteDistance, RouteRealTimeDistance, RouteRealTimeInstructions));
                            });
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new Viewers(RouteName, Role, RouteRate, RouteCost, RouteDuration,
                                    RouteDistance, RouteRealTimeDistance, RouteRealTimeInstructions));
                            });
                        }
                    }
                    else
                    {
                        await page.DisplayAlert("Error", "There isn't any route rate alloted to this trip", "Ok");
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Load Routes using PolyLines
        public async Task<System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Position>> LoadRoutes(string mode, string waypoints)
        {
            try
            {
                var googleDirection = await ApiServices.ServiceClientInstance.GetDirections(originAddress, destinationAddress, mode, waypoints);

                googleDirectionGlobal = googleDirection;

                if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
                {
                    foreach (var item in googleDirection.Routes)
                    {
                        foreach (var item2 in item.Legs)
                        {
                            RouteDistance = item2.Distance.Text;
                            RouteDuration = item2.Duration.Text;

                            var cost = item2.Distance.Value * RouteRate;

                            RouteRate = cost;
                        }
                    }

                    var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));
                    return positions;
                }
                else
                {
                    await page.DisplayAlert("Alert", "Could not load route", "Ok");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region Models
        string origin = string.Empty;
        public string Origin
        {
            get { return origin; }
            set { SetProperty(ref origin, value); }
        }
        
        string routeName = string.Empty;
        public string RouteName
        {
            get { return routeName; }
            set { SetProperty(ref routeName, value); }
        }

        string role = string.Empty;
        public string Role
        {
            get { return role; }
            set { SetProperty(ref role, value); }
        }

        decimal routeRate = 0m;
        public decimal RouteRate
        {
            get { return routeRate; }
            set { SetProperty(ref routeRate, value); }
        }

        decimal routeCost = 0m;
        public decimal RouteCost
        {
            get { return routeCost; }
            set { SetProperty(ref routeCost, value); }
        }

        string destination = string.Empty;
        public string Destination
        {
            get { return destination; }
            set { SetProperty(ref destination, value); }
        }

        string routeDuration = string.Empty;
        public string RouteDuration
        {
            get { return routeDuration; }
            set { SetProperty(ref routeDuration, value); }
        }

        string routeDistance = string.Empty;
        public string RouteDistance
        {
            get { return routeDistance; }
            set { SetProperty(ref routeDistance, value); }
        }

        string routeRealTimeDistance = string.Empty;
        public string RouteRealTimeDistance
        {
            get { return routeRealTimeDistance; }
            set { SetProperty(ref routeRealTimeDistance, value); }
        }

        string routeRealTimeDuration = string.Empty;
        public string RouteRealTimeDuration
        {
            get { return routeRealTimeDuration; }
            set { SetProperty(ref routeRealTimeDuration, value); }
        }

        string routeRealTimeInstructions = string.Empty;
        public string RouteRealTimeInstructions
        {
            get { return routeRealTimeInstructions; }
            set { SetProperty(ref routeRealTimeInstructions, value); }
        }
        #endregion
    }
}
