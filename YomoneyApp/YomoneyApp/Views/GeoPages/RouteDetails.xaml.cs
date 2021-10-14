using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels;

namespace YomoneyApp.Views.GeoPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteDetails : ContentPage
    {
        MapPageViewModel mapPageViewModel;

        public RouteDetails(string routeName, string role, decimal routeRate, decimal routeCost, string routeDuration, string routeDistance)
        {
            InitializeComponent();
            BindingContext = mapPageViewModel = new MapPageViewModel(this);

            if (!String.IsNullOrEmpty(routeName))
            {
                mapPageViewModel.RouteName = routeName;
            }

            if (!String.IsNullOrEmpty(role))
            {
                mapPageViewModel.Role = role;
            }

            if (!String.IsNullOrEmpty(routeRate.ToString()))
            {
                mapPageViewModel.RouteRate = routeRate;
            }

            if (!String.IsNullOrEmpty(routeCost.ToString()))
            {
                mapPageViewModel.RouteCost = routeCost;
            }

            if (!String.IsNullOrEmpty(routeDuration))
            {
                mapPageViewModel.RouteDuration = routeDuration;
            }

            if (!String.IsNullOrEmpty(routeDistance))
            {
                mapPageViewModel.RouteDistance = routeDistance;
            }           
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}