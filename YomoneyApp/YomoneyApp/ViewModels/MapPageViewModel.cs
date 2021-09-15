using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using YomoneyApp.Models;
using YomoneyApp.Services;

namespace YomoneyApp.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        public MapPageViewModel(Page page) : base(page)
        {
        }

        public class YoAppLocations
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        internal async Task<List<YoAppLocations>> LoadDestinationLocations()
        {
            //Call the api to get the locations 

            //This are the hardcoded data
            List<YoAppLocations> yoAppLocations = new List<YoAppLocations>
            {
                new YoAppLocations{Latitude = -17.79312890651072, Longitude=31.062019541904903},
                new YoAppLocations{Latitude = -17.78602890003136,Longitude=31.071782782661366},
                new YoAppLocations{Latitude = -17.780052417002363,Longitude=31.078541949357714},
                new YoAppLocations{Latitude = -17.762867637270315,Longitude=31.050346568343098},
            };

            return yoAppLocations;
        }

            public List<YoAppLocations> yoAppLocations = new List<YoAppLocations>
            {
                 new YoAppLocations{Latitude = -17.79312890651072, Longitude=31.062019541904903},
                 new YoAppLocations{Latitude = -17.762867637270315, Longitude=31.050346568343098}
            };

        internal async Task<System.Collections.Generic.List<Xamarin.Forms.GoogleMaps.Position>> LoadRoute()
        {
            try
            {
                // Load the Original Point and the Destination Point from the Server

                var googleDirection = await ApiServices.ServiceClientInstance.GetDirections(Convert.ToString(yoAppLocations[0].Latitude), Convert.ToString(yoAppLocations[0].Longitude), 
                    Convert.ToString(yoAppLocations[1].Latitude), Convert.ToString(yoAppLocations[1].Longitude));

                if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
                {
                    var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));
                    return positions;
                }
                else
                {
                    await page.DisplayAlert("Alert", "Add your payment method inside the Google Maps console.", "Ok");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
