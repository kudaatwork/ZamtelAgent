using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.DateTime;
using RetailKing.Models;
using System.Net.Http;
using Newtonsoft.Json;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;
using YomoneyApp.ViewModels.Login;
using YomoneyApp.Models.Questions;
using System.Text.RegularExpressions;
using FluentValidation;
using System.Net;
using System.Linq;
using Xamarin.Essentials;
using System.Threading;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Models.PlacesModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xamarin.Forms.GoogleMaps;

namespace YomoneyApp.ViewModels.Geo
{
    [System.ComponentModel.DesignTimeVisible(false)]
    public class GeoLocationViewModel : ViewModelBase
    {
        public GeoLocationViewModel(Page page) : base(page)
        {
            
        }

        Command getCurrentLocationComand;
        public Command GetCurrentLocationComand
        {
            get
            {
                return getCurrentLocationComand ??
                    (getCurrentLocationComand = new Command(async () => await ExecuteGetCurrentLocationComand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteGetCurrentLocationComand()
        {
            if (IsBusy)
                return;

            Message = "Creating Co-ordinates...";
            IsBusy = true;
            getCurrentLocationComand?.ChangeCanExecute();

           // await page.Navigation.PushAsync(GeoLocationPage(Position));   
        }              

        CancellationTokenSource cts;

        #region LocationMethods
        async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                cts = new CancellationTokenSource();

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                   // await page.DisplayAlert("Geolocation Co-ordinates", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "OK");
                    Position = new Position(location.Latitude, location.Longitude);

                   // await page.Navigation.PushAsync(new GeoLocationPage(Position));
                    //Location = location.
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await page.DisplayAlert("Geolocation Error!", "Device not supported", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await page.DisplayAlert("Geolocation Error!", "Device not supported", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await page.DisplayAlert("Geolocation Error!", "Provide the necessary permissions", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await page.DisplayAlert("Geolocation Error!", "Unable to get location", "OK");
            }
        }

        //protected override void OnDisappearing()
        //{
        //    if (cts != null && !cts.IsCancellationRequested)
        //        cts.Cancel();
        //    base.OnDisappearing();
        //}
        #endregion

        Position position;
        public Position Position
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        string message = "Loading...";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        string address = string.Empty;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

        string placename = string.Empty;
        public string PlaceName
        {
            get { return placename; }
            set { SetProperty(ref placename, value); }
        }
    }
}
