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
        HomeViewModel viewModel;

        public RouteDetails(string routeName, string role, decimal routeRate, decimal routeCost, string routeDuration, string routeDistance)
        {
            InitializeComponent();
            BindingContext = viewModel = new HomeViewModel(this);

            if (!String.IsNullOrEmpty(routeName))
            {
                viewModel.RouteName = routeName;
            }

            if (!String.IsNullOrEmpty(role))
            {
                viewModel.Role = role;
            }

            if (!String.IsNullOrEmpty(routeRate.ToString()))
            {
                viewModel.RouteRate = routeRate;
            }

            if (!String.IsNullOrEmpty(routeCost.ToString()))
            {
                viewModel.RouteCost = routeCost;
            }

            if (!String.IsNullOrEmpty(routeDuration))
            {
                viewModel.RouteDuration = routeDuration;
            }

            if (!String.IsNullOrEmpty(routeDistance))
            {
                viewModel.RouteDistance = routeDistance;
            }           
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}