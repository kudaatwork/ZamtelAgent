using MvvmHelpers;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YomoneyApp.Models;
using YomoneyApp.Popups;
using YomoneyApp.Services;
using YomoneyApp.Utils;

namespace YomoneyApp.ViewModels.Countries
{
    public class CountryPickerViewModel : BaseViewModel
    {
        #region Fields

        private CountryModel _selectedCountry;
        CancellationTokenSource cts;
        string HostDomain = "http://192.168.100.150:5000";
        //AccountViewModel viewModel;
        
        public ObservableRangeCollection<CountriesModel> CountriesList { get; set; }

        #endregion Fields

        #region Constructors

        public CountryPickerViewModel(Page page): base(page)
        {
            Title = "Active Country";
            ShowPopupCommand = new Command(async _ => await ExecuteShowPopupCommand());
            CountrySelectedCommand = new Command(country => ExecuteCountrySelectedCommand(country as CountryModel));
            CountriesList = new ObservableRangeCollection<CountriesModel>();
            //viewModel = new AccountViewModel(this);
        }

        #endregion Constructors

        #region Properties

        public CountryModel SelectedCountry
        {
            get => _selectedCountry;
            set => SetProperty(ref _selectedCountry, value);
        }

        #endregion Properties

        #region Commands

        public ICommand ShowPopupCommand { get; }
        public ICommand CountrySelectedCommand { get; }

        #endregion Commands

        #region Get Current Country Location
        private Command getCurrentActiveCountryCommand;

        public Command GetCurrentActiveCountryCommand
        {
            get
            {
                return getCurrentActiveCountryCommand ??
                    (getCurrentActiveCountryCommand = new Command(async () => await ExecuteGetCurrentActiveCountryCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetCurrentActiveCountryCommand()
        {
            IsBusy = true;
            Message = "Loading Active Country...";

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0100";
                trn.ProcessingCode = "200000";

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";

                HttpClient client = new HttpClient();

                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.Country != null)
                    {
                        SelectedCountry.CountryCode = response.Country;
                        SelectedCountry = CountryUtils.GetCountryModelByName(SelectedCountry.CountryName);

                        IsBusy = false;
                    }
                    else
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));

                        cts = new CancellationTokenSource();

                        var location = await Geolocation.GetLocationAsync(request, cts.Token);

                        if (location != null)
                        {
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
                                    $"Thoroughfare:    {placemark.Thoroughfare}\n";

                                 SelectedCountry = CountryUtils.GetCountryModelByName(placemark.CountryName);

                                 IsBusy = false;
                            }
                        }
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception

                //await page.DisplayAlert("Error!", "The location feature is not supported on the device", "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception

                //await page.DisplayAlert("Error!", "The location feature is not enabled on the device", "Ok");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception

                //await page.DisplayAlert("Error!", "The location feature is not permitted on the device", "Ok");
            }
            catch (Exception ex)
            {
                // Unable to get location

                //await page.DisplayAlert("Error!", "Unable to get the loaction on this device", "Ok");
            }
        }
        #endregion

        #region Submit Acive Country
        private Command submitActiveCountryCommand;

        public Command SubmitActiveCountryCommand
        {
            get
            {
                return submitActiveCountryCommand ??
                    (submitActiveCountryCommand = new Command(async () => await ExecuteSubmitActiveCountryCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteSubmitActiveCountryCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            Message = "Submitting Active Country...";

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0100";
                trn.ProcessingCode = "230000";
                trn.Country = SelectedCountry.CountryCode;

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=" + trn.MTI;
                Body += "&Country=" + trn.Country;

                HttpClient client = new HttpClient();

                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "SignedIn")
                    {
                        //IsCountryUpdated = true;
                        //viewModel.CallAlert();
                        IsBusy = false;
                        Message = "";

                        await page.DisplayAlert("Success!", "Active Country Updated Successfully! You will now be sig services withing the Active Country you've just selected", "Ok");
                    }
                    else
                    {
                        IsBusy = false;
                        Message = "";

                        await page.DisplayAlert("Erorr!", "Sorry, there has been an error in updating your Active Country", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Message = "";

                await page.DisplayAlert("Erorr!", "Sorry, there has been an error in updating your Active Country", "Ok");
            }            
        }
        #endregion

        #region Get Countries
        private Command getCountriesCommand;

        public Command GetCountriesCommand
        {
            get
            {
                return getCountriesCommand ??
                    (getCountriesCommand = new Command(async () => await ExecuteGetCountriesCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetCountriesCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            Message = "Getting countries...";

            try
            {               
                HttpClient client = new HttpClient();

                var myContent = "";
                string paramlocal = string.Format("https://restcountries.com/v2/all?fields=name,callingCodes,flags,alpha2Code", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var countries = JsonConvert.DeserializeObject<List<CountriesModel>>(result);
                    CountriesList.ReplaceRange(countries);
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Message = "";

                await page.DisplayAlert("Error!", "Sorry, there has been an error in retrieving Countries from the server!", "Ok");
            }
        }
        #endregion

        #region Private Methods

        public Task ExecuteShowPopupCommand()
        {
            var popup = new ChooseCountryPopup(SelectedCountry)
            {
                CountrySelectedCommand = CountrySelectedCommand
            };

            return Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
        }

        public void ExecuteCountrySelectedCommand(CountryModel country)
        {
            SelectedCountry = country;
        }

        #endregion Private Methods

        string message = "Loading...";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        bool isCountryUpdated = false;
        public bool IsCountryUpdated
        {
            get { return isCountryUpdated; }
            set { SetProperty(ref isCountryUpdated, value); }
        }

        

    }
}
