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
using MvvmHelpers;
using YomoneyApp.Models;
using SQLite;
using static YomoneyApp.Storage.DbConnection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace YomoneyApp.ViewModels.Geo
{
    [System.ComponentModel.DesignTimeVisible(false)]

    public class MainViewModel : INotifyPropertyChanged
    {
        //public ICommand CalculateRouteCommand { get; set; }
        //public ICommand UpdatePositionCommand { get; set; }

        //public ICommand LoadRouteCommand { get; set; }
        //public ICommand StopRouteCommand { get; set; }
        IGoogleMapsApiService googleMapsApi = new GoogleMapsApiService();

        bool _hasRouteRunning;
        string _originLatitud;
        string _originLongitud;
        string _destinationLatitud;
        string _destinationLongitud;

        GooglePlaceAutoCompletePrediction _placeSelected;
        public GooglePlaceAutoCompletePrediction PlaceSelected
        {
            get
            {
                return _placeSelected;
            }
            set
            {
                _placeSelected = value;
                if (_placeSelected != null)
                    GetPlaceDetailCommand.Execute(_placeSelected);
            }
        }

        public ICommand FocusOriginCommand { get; set; }
        public ICommand GetPlacesCommand { get; set; }
        public ICommand GetPlaceDetailCommand { get; set; }

        public ObservableCollection<GooglePlaceAutoCompletePrediction> Places { get; set; }
        public ObservableCollection<GooglePlaceAutoCompletePrediction> RecentPlaces { get; set; } = new ObservableCollection<GooglePlaceAutoCompletePrediction>();

        public bool ShowRecentPlaces { get; set; }
        bool _isAddressFocused = true;

        string addressText;
        public string AddressText
        {
            get
            {
                return addressText;
            }
            set
            {
                addressText = value;

                if (!string.IsNullOrEmpty(addressText))
                {
                    _isAddressFocused = true;
                    GetPlacesCommand.Execute(addressText);
                }
            }
        }

        //string _originText;
        //public string OriginText
        //{
        //    get
        //    {
        //        return _originText;
        //    }
        //    set
        //    {
        //        _originText = value;
        //        if (!string.IsNullOrEmpty(_originText))
        //        {
        //            _isPickupFocused = false;
        //            GetPlacesCommand.Execute(_originText);
        //        }
        //    }
        //}

        public MainViewModel()
        {
            //LoadRouteCommand = new Command(async () => await LoadRoute());
            //StopRouteCommand = new Command(StopRoute);
            GetPlacesCommand = new Command<string>(async (param) => await GetPlacesByName(param));
            GetPlaceDetailCommand = new Command<GooglePlaceAutoCompletePrediction>(async (param) => await GetPlacesDetail(param));
        }

        //public async Task LoadRoute()
        //{
        //    var positionIndex = 1;
        //    var googleDirection = await googleMapsApi.GetDirections(_originLatitud, _originLongitud, _destinationLatitud, _destinationLongitud);
        //    if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
        //    {
        //        var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));
        //        CalculateRouteCommand.Execute(positions);

        //        _hasRouteRunning = true;

        //        //Location tracking simulation
        //        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        //        {
        //            if (positions.Count > positionIndex && _hasRouteRunning)
        //            {
        //                UpdatePositionCommand.Execute(positions[positionIndex]);
        //                positionIndex++;
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        });
        //    }
        //    else
        //    {
        //        await App.Current.MainPage.DisplayAlert(":(", "No route found", "Ok");
        //    }

        //}
        //public void StopRoute()
        //{
        //    _hasRouteRunning = false;
        //}

        public async Task GetPlacesByName(string placeText)
        {
            var places = await googleMapsApi.GetPlaces(placeText);

            var placeResult = places.AutoCompletePlaces;

            if (placeResult != null && placeResult.Count > 0)
            {
                Places = new ObservableCollection<GooglePlaceAutoCompletePrediction>(placeResult);
            }

            ShowRecentPlaces = (placeResult == null || placeResult.Count == 0);
        }

        public async Task GetPlacesDetail(GooglePlaceAutoCompletePrediction placeA)
        {
            var place = await googleMapsApi.GetPlaceDetails(placeA.PlaceId);

            if (place != null)
            {
                AddressText = place.Name;
                _originLatitud = $"{place.Latitude}";
                _originLongitud = $"{place.Longitude}";
                _isAddressFocused = false;
                FocusOriginCommand.Execute(null);

                //else
                //{
                //    _destinationLatitud = $"{place.Latitude}";
                //    _destinationLongitud = $"{place.Longitude}";

                //    RecentPlaces.Add(placeA);

                //    if (_originLatitud == _destinationLatitud && _originLongitud == _destinationLongitud)
                //    {
                //        await App.Current.MainPage.DisplayAlert("Error", "Origin route should be different than destination route", "Ok");
                //    }
                //    else
                //    {
                //        //LoadRouteCommand.Execute(null);
                //        await App.Current.MainPage.Navigation.PopAsync(false);
                //        CleanFields();
                //    }

                //}
            }
        }

        void CleanFields()
        {
            AddressText = AddressText = string.Empty;
            ShowRecentPlaces = true;
            PlaceSelected = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
