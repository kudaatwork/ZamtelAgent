using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MvvmHelpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using YomoneyApp.Views.Menus;
using YomoneyApp.Views.Profile.Loyalty;
using RetailKing.Models;
using YomoneyApp.Views.TransactionHistory;
using YomoneyApp.Services;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Webview;
using YomoneyApp.Views.Spend;
using YomoneyApp.Views.Loyalty;
using YomoneyApp.Views.Promotions;
using YomoneyApp.Views.Chat;
using Xamarin.Essentials;
using Plugin.Toasts;
using YomoneyApp.Models.Image;
using YomoneyApp.Models;
using System.Windows.Input;
using System.ComponentModel;

namespace YomoneyApp
{
    public class PromotionsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        IGoogleMapsApiService googleMapsApi = new GoogleMapsApiService();

        string _originLatitud;
        string _originLongitud;

        GooglePlaceAutoCompletePrediction _placeSelected;
        public GooglePlaceAutoCompletePrediction PlaceSelected
        {
            get
            {
                return _placeSelected;
            }
            set
            {
                IsAdvert = false;
                _placeSelected = value;
                if (_placeSelected != null)
                    GetPlaceDetailCommand.Execute(_placeSelected);
            }
        }

        public ICommand FocusOriginCommand { get; set; }
        public ICommand GetPlacesCommand { get; set; }
        public ICommand GetPlaceDetailCommand { get; set; }

        public ObservableRangeCollection<GooglePlaceAutoCompletePrediction> Places { get; set; }
        public ObservableCollection<GooglePlaceAutoCompletePrediction> RecentPlaces { get; set; } = new ObservableCollection<GooglePlaceAutoCompletePrediction>();
        public ObservableRangeCollection<MenuItem> Categories { get; set; }
        public bool ShowRecentPlaces { get; set; }
        // bool _isAddressFocused = true;

        string location;
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                //IsAdvert = true;
                location = value;

                if (!string.IsNullOrEmpty(location))
                {
                    // _isAddressFocused = true;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
                    
                    if (isShow == false)
                    {
                        GetPlacesCommand.Execute(location);                        
                    }                    
                }
            }
        }

        string HostDomain = "http://192.168.100.150:5000";
        //string ProcessingCode = "350000";
        public ObservableRangeCollection<MenuItem> ServiceProviders { get; set; }
        public ObservableRangeCollection<MenuItem> ServiceOptions { get; set; }
        public ObservableRangeCollection<MenuItem> ServiceList { get; set; }
        public ObservableRangeCollection<MenuItem> SearchResults { get; set; }
        public MenuItem PromoDetail { get; set; }
        public bool ForceSync { get; set; }

        string Latitude = "";
        string Longitude = "";

        GooglePlaceAutoCompletePrediction prediction = new GooglePlaceAutoCompletePrediction();
        GooglePlaceAutoCompletePrediction prediction2 = new GooglePlaceAutoCompletePrediction();
        List<GooglePlaceAutoCompletePrediction> googlePlaceAutoCompletePredictions = new List<GooglePlaceAutoCompletePrediction>();

        public PromotionsViewModel(Page page, MenuItem selected) : base(page)
        {
            Title = selected.Title;
            SearchResults = new ObservableRangeCollection<MenuItem>();
            Categories = new ObservableRangeCollection<MenuItem>();
            ServiceList = new ObservableRangeCollection<MenuItem>();
            ServiceOptions = new ObservableRangeCollection<MenuItem>();
            ServiceProviders = new ObservableRangeCollection<MenuItem>();
            PromoDetail = new MenuItem();
            Places = new ObservableRangeCollection<GooglePlaceAutoCompletePrediction>();
            prediction.StructuredFormatting = new StructuredFormatting();
            prediction2.StructuredFormatting = new StructuredFormatting();

            prediction.Description = "First Item";
            prediction.StructuredFormatting.MainText = "First Main Text";
            prediction.StructuredFormatting.SecondaryText = "Second Main Text";

            prediction2.Description = "First Item";
            prediction2.StructuredFormatting.MainText = "Taapa Second Main Text";
            prediction2.StructuredFormatting.SecondaryText = "Ikozvino taapa Second Main Text";

            //IsAdvert = true;

            //googlePlaceAutoCompletePredictions.Add(prediction);

            //Places.ReplaceRange(googlePlaceAutoCompletePredictions);

            ShowLocation = true;

            GetPlacesCommand = new Command<string>(async (param) => await GetPlacesByName(param));
            GetPlaceDetailCommand = new Command<GooglePlaceAutoCompletePrediction>(async (param) => await GetPlacesDetail(param));
        }

        public Action<MenuItem> ItemSelected { get; set; }

        MenuItem selectedOption;
        public MenuItem SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = value;
                OnPropertyChanged("SelectedOption");
                if (selectedOption == null)
                    return;

                if (ItemSelected == null)
                {
                    page.Navigation.PushAsync(new ProviderServices(selectedOption));
                    selectedOption = null;
                    SelectedOption = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedOption);
                }
            }
        }

        MenuItem selectedVariation;
        public MenuItem SelectedVariation
        {
            get { return selectedVariation; }
            set
            {
                selectedVariation = value;
                OnPropertyChanged("SelectedVariation");
                if (selectedVariation == null)
                    return;

                if (ItemSelected == null)
                {
                    page.Navigation.PushAsync(new ServiceCategories(selectedVariation));
                    selectedVariation = null;
                    SelectedVariation = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedVariation);
                }
            }
        }

        MenuItem selectedCategory;
        public MenuItem SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
                if (selectedCategory == null)
                    return;

                try
                {
                    if (ItemSelected == null)
                    {
                        if (selectedCategory.Title == "REWARD SCHEMES" || selectedCategory.Title == "LUCKY DRAW")
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new ServiceProviders(selectedCategory));
                            });

                            // page.Navigation.PushAsync(new ServiceProviders(selectedCategory));
                            // selectedCategory = null;
                            // SelectedCategory = null;

                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new ProviderPromotions(selectedCategory));
                            });

                            //page.Navigation.PushAsync(new ProviderPromotions(selectedCategory));
                            // selectedCategory = null;
                            // SelectedCategory = null;
                        }
                    }
                    else
                    {
                        ItemSelected.Invoke(selectedCategory);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        MenuItem selectedService;
        public MenuItem SelectedService
        {
            get { return selectedService; }
            set
            {
                selectedService = value;
                OnPropertyChanged("SelectedService");
                if (selectedService == null)
                    return;

                if (ItemSelected == null)
                {
                    if (selectedService.Title == "REWARD SCHEMES")
                    {
                        page.Navigation.PushAsync(new ServiceActions(selectedService));
                        selectedService = null;
                        SelectedService = null;
                    }
                    else
                    {
                        page.Navigation.PushAsync(new SharePage(selectedService));
                        selectedService = null;
                        SelectedService = null;
                    }
                    // RedeemSection = null;  
                }
                else
                {
                    ItemSelected.Invoke(selectedService);
                }
            }
        }

        MenuItem renderService;
        public MenuItem RenderService
        {
            get { return renderService; }
            set
            {
                renderService = value;
                OnPropertyChanged("RenderService");
                if (renderService == null)
                    return;

                if (ItemSelected == null)
                {
                    RenderServiceAction(renderService);
                    RenderService = null;
                    renderService = null;
                }
                else
                {
                    ItemSelected.Invoke(renderService);
                }
            }
        }

        MenuItem selectedAction;
        public MenuItem SelectedAction
        {
            get { return selectedAction; }
            set
            {
                selectedAction = value;
                OnPropertyChanged("SelectedAction");
                if (selectedAction == null)
                    return;

                if (ItemSelected == null)
                {

                    switch (selectedAction.Section)
                    {
                        case "Loyalty":
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;
                        case "Service":
                            page.Navigation.PushAsync(new ServiceActions(selectedAction));
                            break;
                        case "Vouchers":
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;
                        case "Payment":
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;

                    }
                    // page.Navigation.PushAsync(new SelectPage(selectedAction));
                    SelectedAction = null;
                    selectedAction = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedAction);
                }
            }
        }

        ChatMessage chatMessage;

        #region Get Places
        public async Task GetPlacesByName(string placeText)
        {
            IsAdvert = true;

            var places = await googleMapsApi.GetPlaces(placeText);

            var placeResult = places.AutoCompletePlaces;

            if (placeResult != null && placeResult.Count > 0)
            {
                Places.ReplaceRange(placeResult);

                //googlePlaceAutoCompletePredictions.Add(prediction2);

                //Places.ReplaceRange(googlePlaceAutoCompletePredictions);
            }

            ShowRecentPlaces = (placeResult == null || placeResult.Count == 0);
        }

        public async Task GetPlacesDetail(GooglePlaceAutoCompletePrediction placeA)
        {
            IsAdvert = false;
            isShow = true;

            var place = await googleMapsApi.GetPlaceDetails(placeA.PlaceId);

            if (place != null)
            {
                Location = place.Address;
                minLatitude = Convert.ToDouble(place.Latitude);
                minLongitude = Convert.ToDouble(place.Longitude);
                Places.Clear();
                isShow = false;
                showLocation = false;
                showLabel = true;
                //_isAddressFocused = false;
                //FocusOriginCommand.Execute(null);

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

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region model

        bool showSearchList = false;
        public bool ShowSearchList
        {
            get { return showSearchList; }
            set
            {
                SetProperty(ref showSearchList, value);
            }
        }

        bool hideSearchList = true;
        public bool HideSearchList
        {
            get { return hideSearchList; }
            set
            {
                SetProperty(ref hideSearchList, value);
            }
        }

        string supplier;
        public string Supplier
        {
            get { return supplier; }
            set
            {
                SetProperty(ref supplier, value);
            }
        }

        long points;
        public long Points
        {
            get { return points; }
            set
            {
                SetProperty(ref points, value);
            }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        double minLatitude = 0;
        public double MinLatitude
        {
            get { return minLatitude; }
            set {
                minLatitude = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinLatitude"));
            }
        }

        double minLongitude = 0;
        public double MinLongitude
        {
            get { return minLongitude; }
            set {
                minLongitude = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinLongitude"));
            }
        }

        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set
            {
                //SetProperty(ref isAdvert, value);
                message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
            }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        string adPosition = string.Empty;
        public string AdPosition
        {
            get { return adPosition; }
            set { SetProperty(ref adPosition, value); }
        }

        string adtype = string.Empty;
        public string Adtype
        {
            get { return adtype; }
            set { SetProperty(ref adtype, value); }
        }

        string linkParameterName = string.Empty;
        public string LinkParameterName
        {
            get { return linkParameterName; }
            set { SetProperty(ref linkParameterName, value); }
        }

        string sex = string.Empty;
        public string Sex
        {
            get { return sex; }
            set { SetProperty(ref sex, value); }
        }

        bool isAdvert = false;
        public bool IsAdvert
        {
            get { return isAdvert; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isAdvert = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAdvert"));
            }
        }

        bool showLabel = false;
        public bool ShowLabel
        {
            get { return showLabel; }
            set
            {
                //SetProperty(ref isAdvert, value);
                showLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowLabel"));
            }
        }

        bool showLocation = false;
        public bool ShowLocation
        {
            get { return showLocation; }
            set
            {
                //SetProperty(ref isAdvert, value);
                showLocation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShowLocation"));
            }
        }

        bool isShow = false;
        public bool IsShow
        {
            get { return isShow; }
            set { SetProperty(ref isShow, value); }
        }

        bool hasParameter = false;
        public bool HasParameter
        {
            get { return hasParameter; }
            set
            {
                hasParameter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasParameter"));
            }
        }

        int minAge = 0;
        public int MinAge
        {
            get { return minAge; }
            set { SetProperty(ref minAge, value); }
        }

        DateTime Date = DateTime.Now.Date;
        public DateTime ExpireryDate
        {
            get { return Date; }
            set { SetProperty(ref Date, value); }
        }

        string currency = string.Empty;
        public string Currency
        {
            get { return currency; }
            set { SetProperty(ref currency, value); }
        }

        Nullable<decimal> dailymax = null;
        public Nullable<decimal> DailyMax
        {
            get { return dailymax; }
            set { SetProperty(ref dailymax, value); }
        }

        int maxage = 0;
        public int MaxAge
        {
            get { return maxage; }
            set { SetProperty(ref maxage, value); }
        }

        Nullable<int> radius = null;
        public Nullable<int> Radius
        {
            get { return radius; }
            set { SetProperty(ref radius, value); }
        }

        string siteUrl = string.Empty;
        public string SiteUrl
        {
            get { return siteUrl; }
            set { SetProperty(ref siteUrl, value); }
        }

        #endregion

        #region load other Services
        public void GetOtherServices(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {

                GetVariationsCommand.Execute(null);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            }
        }

        private Command getVariationsCommand;

        public Command GetVariationsCommand
        {
            get
            {
                return getVariationsCommand ??
                    (getVariationsCommand = new Command(async () => await ExecuteGetVariationsCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetVariationsCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetVariationsCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Service Variations";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = 1;
                trn.ServiceProvider = itm.Section;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                    foreach (var gtm in servics)
                    {

                        if (gtm.Image == null)
                        {

                            gtm.Image = HostDomain + "/Content/Spani/Images/myServices.jpg";
                        }

                    }

                    if (servics.Count > 0)
                    {
                        ServiceOptions.ReplaceRange(servics);
                    }
                    else
                    {

                        await page.Navigation.PushModalAsync(new CommingSoon());

                        await page.Navigation.PopAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetVariationsCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }


        #endregion

        #region Submit Promotion 
        private Command submitPromotionCommand;

        public Command SubmitPromotionCommand
        {
            get
            {
                return submitPromotionCommand ??
                    (submitPromotionCommand = new Command(async () => await ExecuteSubmitPromotionCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteSubmitPromotionCommand(MenuItem itm)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            SubmitPromotionCommand.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Service Variations";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = 1;
                trn.ServiceProvider = itm.Section;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                    foreach (var gtm in servics)
                    {

                        if (gtm.Image == null)
                        {

                            gtm.Image = HostDomain + "/Content/Spani/Images/myServices.jpg";
                        }

                    }

                    if (servics.Count > 0)
                    {
                        ServiceOptions.ReplaceRange(servics);
                    }
                    else
                    {

                        await page.Navigation.PushModalAsync(new CommingSoon());

                        await page.Navigation.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.InnerException + ex.StackTrace);
            }

        }

        #endregion

        #region load Promotion Categories
        public void GetPromotionCategories(MenuItem mm)
        {
            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetCategoriesCommand.Execute(null);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            }
        }

        private Command getCategoriesCommand;

        public Command GetCategoriesCommand
        {
            get
            {
                return getCategoriesCommand ??
                    (getCategoriesCommand = new Command(async () => await ExecuteGetCategoriesCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetCategoriesCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetCategoriesCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Service Categories";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = 1;
                trn.ServiceProvider = itm.Section;
                trn.Note = itm.Title;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                    foreach (var gtm in servics)
                    {

                        if (gtm.Image == null)
                        {
                            if (itm.Image != null)
                            {
                                gtm.Image = itm.Image;
                            }
                            else
                            {
                                gtm.Image = HostDomain + "/Content/Spani/Images/Campaign.ong";
                            }
                        }

                    }

                    if (servics.Count > 0)
                    {
                        ServiceOptions.ReplaceRange(servics);
                    }
                    else
                    {
                        await page.Navigation.PushModalAsync(new CommingSoon());
                        await page.Navigation.PopAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetVariationsCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }

        #endregion

        #region load serviceProviders
        public void GetSeledtedValues(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {

                GetProviderCommand.Execute(null);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            }
        }

        private Command getProviderCommand;

        public Command GetProviderCommand
        {
            get
            {
                return getProviderCommand ??
                    (getProviderCommand = new Command(async () => await ExecuteGetProviderCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetProviderCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetProviderCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Service Providers";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;

                trn.Note = itm.Title;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    var provs = servics.GroupBy(u => u.Title).ToList();
                    List<MenuItem> ProvList = new List<MenuItem>();
                    foreach (var gtm in provs)
                    {
                        var i = gtm.FirstOrDefault();
                        // if (i.Note != null)
                        // {
                        MenuItem mn = new MenuItem();

                        mn.Title = i.Note;
                        if (i.Image == null)
                        {
                            if (itm.Image != null)
                            {
                                mn.Image = itm.Image;
                            }
                            else if (trn.TransactionType == 1)
                            {
                                mn.Image = HostDomain + "/Content/Spani/Images/myServices.jpg";
                            }
                            else
                            {
                                mn.Image = HostDomain + "/Content/Spani/Images/" + trn.TransactionType + ".jpg";
                            }
                        }
                        else
                        {
                            char[] delimite = new char[] { '~' };
                            string[] parts = i.Image.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                            mn.Image = HostDomain + parts[0];
                        }
                        mn.SupplierId = i.SupplierId;
                        mn.TransactionType = i.TransactionType;
                        mn.Note = trn.Note;
                        mn.Description = i.Description;
                        ProvList.Add(mn);
                    }
                    //}
                    if (ProvList.Count > 0)
                    {
                        ServiceOptions.ReplaceRange(ProvList);
                    }
                    else
                    {

                        await page.Navigation.PushModalAsync(new CommingSoon());
                        await page.Navigation.PopAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                getProviderCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }

        #endregion

        #region load services
        public void GetSelectedProvider(MenuItem mm)
        {

            selectedOption = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedOption == null)
                return;

            if (ItemSelected == null)
            {

                GetServicesCommand.Execute(null);
                SelectedOption = null;
            }
            else
            {
                ItemSelected.Invoke(selectedOption);
            }
        }

        private Command getServicesCommand;

        public Command GetServicesCommand
        {
            get
            {
                return getServicesCommand ??
                    (getServicesCommand = new Command(async () => await ExecuteGetServicesCommand(selectedOption), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetServicesCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetActionCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;

                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Supplier Services";
                if (itm.TransactionType == 0)
                {
                    trn.TransactionType = 1;
                }
                else
                {
                    trn.TransactionType = itm.TransactionType;
                }
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Note = itm.Note;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                string img = itm.Image;
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                    foreach (var serv in servics)
                    {
                        if (serv.Image != null)
                        {
                            char[] delimiters = new char[] { '~' };
                            string[] supid = serv.Image.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                            serv.Image = HostDomain + supid[0];
                        }
                        else if (img != null)
                        {
                            serv.Image = img;
                        }
                    }
                    ServiceOptions.ReplaceRange(servics);
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetServicesCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region load Current Promotions
        public void GetCurrentPromotions(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetPromoCommand.Execute(mm);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            };

        }

        private Command getPromoCommand;

        public Command GetPromoCommand
        {
            get
            {
                return getPromoCommand ??
                    (getPromoCommand = new Command(async () => await ExecuteGetPromoCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetPromoCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetActionCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceList.Clear();

                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Supplier Services";

                if (itm.TransactionType == 0)
                {
                    trn.TransactionType = 1;
                }
                else
                {
                    trn.TransactionType = itm.TransactionType;
                }
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = "PROMOTION LIST";
                trn.AgentCode = itm.SupplierId;
                trn.Note = "PROMOTION LIST";
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);

                string img = itm.Image;

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                    foreach (var it in servics)
                    {
                        if (it.MediaType.Trim() == "Image")
                        {
                            it.IsAdvert = true;
                            it.IsNotAdvert = false;
                            it.Media = "http://192.168.100.150:5000/Content/notify/notify.mp3";
                        }
                        else
                        {
                            it.IsAdvert = false;
                            it.IsNotAdvert = true;
                            it.Media = it.Image;
                            it.Image = "InsuranceBanner.png";
                        }

                        if (string.IsNullOrEmpty(it.UserImage))
                        {
                            it.UserImage = "http://192.168.100.150:5000/Content/Administration/images/user.png"; ;
                        }
                    }

                    ServiceList.ReplaceRange(servics);
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetServicesCommand.ChangeCanExecute();
            }

            //if (showAlert)
            //  await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region Load Current User Adverts/Promotions

        public void GetCurrentUserPromotions(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetUserPromotionsCommand.Execute(mm);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            };
        }

        private Command getUserPromotionsCommand;

        public Command GetUserPromotionsCommand
        {
            get
            {
                return getUserPromotionsCommand ??
                    (getUserPromotionsCommand = new Command(async () => await ExecuteGetUserPromotionsCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetUserPromotionsCommand(MenuItem itm)
        {
            //if (IsBusy)
            //    return;
            //if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
           // GetActionCommand.ChangeCanExecute();
            var showAlert = false;

            try
            {
                ServiceList.Clear();

                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerMSISDN = uname;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Supplier Services";

                if (itm.TransactionType == 0)
                {
                    trn.TransactionType = 1;
                }
                else
                {
                    trn.TransactionType = itm.TransactionType;
                }

                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = "USER PROMOTION LIST";
                trn.AgentCode = itm.SupplierId;
                trn.Note = "PROMOTION LIST";

                string Body = "";

                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();

                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);

                string img = itm.Image;

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (string.IsNullOrEmpty(response.Currency))
                    {
                        response.Currency = "ZWL";
                    }

                    var balance = Math.Round(Convert.ToDecimal(response.Balance), 2);

                    Amount = response.Currency + " " + balance;

                    if (!string.IsNullOrEmpty(response.Narrative))
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);

                        if (servics.Count > 0)
                        {
                            IsNotNullAdverts = true;
                            IsAdvert = true;

                            foreach (var it in servics)
                            {
                                if (string.IsNullOrEmpty(it.MediaType))
                                {
                                    it.IsAdvert = true;
                                    it.IsNotAdvert = false;
                                    it.Media = "http://192.168.100.150:5000/Content/notify/notify.mp3";
                                }
                                else if (it.MediaType.Trim() == "Image")
                                {
                                    it.IsAdvert = true;
                                    it.IsNotAdvert = false;
                                    it.Media = "http://192.168.100.150:5000/Content/notify/notify.mp3";                                    
                                }
                                else
                                {
                                    it.IsAdvert = false;
                                    it.IsNotAdvert = true;
                                    it.Media = it.Image;
                                    //it.Image = "InsuranceBanner.png";
                                }

                                if (string.IsNullOrEmpty(it.UserImage))
                                {
                                    it.UserImage = "http://192.168.100.150:5000/Content/Administration/images/user.png";
                                }

                                if (it.Status.Trim() == "Active")
                                {
                                    it.IsShare = true;
                                    it.ThemeColor = "Green";                                    
                                }
                                else if (it.Status.Trim() == "Disabled")
                                {
                                    it.IsEmptyList = true;
                                    it.ThemeColor = "Orange";                                    
                                }
                                else 
                                {
                                    it.IsShare = true;
                                    it.ThemeColor = "Orange";                                    
                                }
                            }
                        }
                        else
                        {
                            IsNullAdverts = true;
                             //IsNotNullAdverts = false;

                            MenuItem menuItem = new MenuItem();

                            menuItem.Image = "ad1.png";
                            menuItem.MediaType = "Image";
                            menuItem.Note = "YOAPP";
                            menuItem.Section = "PROMOTION";
                            menuItem.SupplierId = "5-0001-0000000";
                            menuItem.Title = "Create your own Promotion";
                            menuItem.TransactionType = 23;
                            menuItem.UserImage = "http://192.168.100.150:5000/Content/Administration/images/user.png";
                            menuItem.IsAdvert = true;
                            menuItem.IsNotAdvert = false;
                            menuItem.Media = "http://192.168.100.150:5000/Content/notify/notify.mp3";

                            servics.Add(menuItem);
                        }

                        ServiceList.ReplaceRange(servics);
                    }
                    else
                    {
                        IsNullAdverts = true;

                        MenuItem menuItem = new MenuItem();
                        List<MenuItem> menuItems = new List<MenuItem>();

                        menuItem.Image = "ad1.png";
                        menuItem.MediaType = "Image";
                        menuItem.Note = "YOAPP";
                        menuItem.Section = "PROMOTION";
                        menuItem.SupplierId = "5-0001-0000000";
                        menuItem.Title = "Create your own Promotion";
                        menuItem.TransactionType = 23;
                        menuItem.UserImage = "http://192.168.100.150:5000/Content/Administration/images/user.png";
                        menuItem.IsAdvert = true;
                        menuItem.IsNotAdvert = false;
                        menuItem.Media = "http://192.168.100.150:5000/Content/notify/notify.mp3";

                        menuItems.Add(menuItem);

                        IsAdvert = true;

                        ServiceList = new ObservableRangeCollection<MenuItem>();

                        ServiceList.ReplaceRange(menuItems);

                        //await page.Navigation.PushModalAsync(new PromotionsNotFound());

                        //await page.Navigation.PopAsync();
                    }                    
                }
                else
                {
                    await page.DisplayAlert("List Adverts Error!", "Unable to gather Adverts from the from the server. Please check your internet connection and try again", "OK");
                }
            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                //GetServicesCommand.ChangeCanExecute();
            }

            if (showAlert)
               await page.DisplayAlert("List Adverts Error!", "We are unable to gather your adverts. Please check your internet connection and try again", "OK");

        }

        #endregion

        #region Get YoApp Services

        public async Task<IEnumerable<MenuItem>> GetYoAppServicesAsync(string note, MenuItem selected)
        {
            if (IsBusy)
                return Categories;
            Message = "Loading YoApp Services...";
            IsBusy = true;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "430000";
                trn.Narrative = "2";
                trn.Note = selected.Section;// "JobSectors";
                trn.Product = note;
                trn.ServiceId = selected.ServiceId;
                trn.TransactionType = selected.TransactionType;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Categories.ReplaceRange(servics);
                    return servics;

                }
                return new List<MenuItem>();

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error!", "Unable to gather billers.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }

        #endregion

        #region Topup Balance
        private Command topupBalanceCommand;

        public Command TopupBalanceCommand
        {
            get
            {
                return topupBalanceCommand ??
                    (topupBalanceCommand = new Command(async () => await ExecuteTopupBalanceCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteTopupBalanceCommand()
        {
            if (IsBusy)
                return;

            // if (ForceSync)
            //Settings.LastSync = DateTime.Now.AddDays(-30);

            if (string.IsNullOrWhiteSpace(Currency) || string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Blank Fields Error!", "Please enter all fields", "OK");
                return;
            }

            var showAlert = false;

            //IsBusy = true;
            //Message = "Processing, please wait...";

            if (sendSms == false)
            {
                PhoneNumber = "";
            }
                        
            MenuItem mn = new YomoneyApp.MenuItem();
            mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
            Category = "TOP BALANCE";

            mn.Title = Category;

            await page.Navigation.PushAsync(new PaymentPage(mn));

            MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (sender, arg) =>
            {
                if (arg == "Payment Success")
                {
                    #region Commented Out Code
                    //IsBusy = true;
                    //Message = "Processing " + mn.Title;
                    //#region getrecharge
                    //try
                    //{
                    //    // if (ServiceOptions != null)
                    //    //   ServiceOptions.Clear();
                    //    List<MenuItem> mnu = new List<MenuItem>();
                    //    TransactionRequest trn = new TransactionRequest();
                    //    AccessSettings acnt = new Services.AccessSettings();
                    //    string pass = acnt.Password;
                    //    string uname = acnt.UserName;
                    //    trn.CustomerAccount = uname + ":" + pass;
                    //    trn.MTI = "0200";
                    //    trn.ProcessingCode = "320000";
                    //    trn.Narrative = "7";
                    //    trn.Note = "Yomoney";
                    //    trn.CustomerData = PhoneNumber;
                    //    trn.TerminalId = "ClientApp";
                    //    trn.TransactionRef = "";
                    //    trn.TransactionType = 8;
                    //    trn.ServiceProvider = "5-0001-0000000";
                    //    trn.Narrative = "Purchase";
                    //    trn.Note = "YomoneySupplier";
                    //    trn.ServiceId = 13; //Categories.Where(u => u.Title == category).FirstOrDefault().ServiceId;
                    //    trn.Product = "1"; //Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                    //    trn.ActionId = 1; //long.Parse(Categories.Where(u => u.Title == category).FirstOrDefault().Id);
                    //    trn.Amount = decimal.Parse(Budget);
                    //    trn.Currency = mn.Currency;
                    //    //trn.CustomerMSISDN = AccountNumber;
                    //    string Body = "";
                    //   // Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    //    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    //    Body += "&AgentCode=" + trn.AgentCode;
                    //    Body += "&Action=Mobile";
                    //    Body += "&TerminalId=" + trn.TerminalId;
                    //    Body += "&TransactionRef=" + trn.TransactionRef;
                    //    Body += "&ServiceId=" + trn.ServiceId;
                    //    Body += "&Product=" + trn.Product;
                    //    Body += "&Amount=" + trn.Amount;
                    //    Body += "&MTI=" + trn.MTI;
                    //    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    //    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    //    Body += "&Narrative=" + trn.Narrative;
                    //    Body += "&CustomerData=" + trn.CustomerData;
                    //    Body += "&Quantity=" + trn.Quantity;
                    //    Body += "&Note=" + trn.Note;
                    //    Body += "&Mpin=" + trn.Mpin;
                    //    Body += "&Currency=" + trn.Currency;
                    //    Body += "&TransactionType=" + trn.TransactionType;
                    //    Body += "&ActionId=" + trn.ActionId;

                    //    HttpClient client = new HttpClient();

                    //    client.Timeout = TimeSpan.FromSeconds(180);

                    //    var myContent = Body;

                    //    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    //    string result = await client.GetStringAsync(paramlocal);

                    //    if (result != "System.IO.MemoryStream")
                    //    {
                    //        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    //        if (response.ResponseCode == "Success" || response.ResponseCode == "00000" || response.ResponseCode == "11304")
                    //        {
                    //            #region SendToken
                    //            if (response.TransactionCode == null && response.CustomerData != null)
                    //            {
                    //                response.TransactionCode = response.CustomerData;
                    //            }
                    //            else if (response.TransactionCode == null)
                    //            {
                    //                response.TransactionCode = "You Have successfully topped up $" + trn.Amount + " to your service balance.";
                    //            }

                    //            var servics = new List<MenuItem>();

                    //            char[] delim = new char[] { '|' };
                    //            char[] delimeter = new char[] { '#' };
                    //            string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);

                    //            if (tokens.Count() > 0)
                    //            {
                    //                for (int i = 0; i < tokens.Length; i++)
                    //                {
                    //                    MenuItem mni = new MenuItem();
                    //                    mni.Description = tokens[i];
                    //                    servics.Add(mni);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                MenuItem mni = new MenuItem();
                    //                mni.Description = response.TransactionCode;
                    //                servics.Add(mni);
                    //            }

                    //            #endregion
                    //         //   Retry = false;
                    //            await page.Navigation.PushAsync(new TockenPage(servics));
                    //         //   Stores.ReplaceRange(servics);
                    //        }
                    //        else
                    //        {

                    //            //Retry = true;
                    //            //IsConfirm = false;
                    //            await page.DisplayAlert("Topup Error!", response.Description, "OK");
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    showAlert = true;
                    //}
                    //finally
                    //{
                    //    IsBusy = false;
                    //    TopupBalanceCommand.ChangeCanExecute();
                    //}

                    //if (showAlert)
                    #endregion

                    await page.DisplayAlert("Payment Successful!", "Your account has been successfully topped up with your amount", "OK");

                    MenuItem mn = new YomoneyApp.MenuItem();
                    mn.Title = "My Promotions";
                    mn.TransactionType = 23;
                    mn.Section = "PROMOTIONS";
                    mn.SupplierId = "All";

                    await page.Navigation.PushAsync(new MyPromotions(mn));
                }
                else
                {
                    IsBusy = false;
                    //await page.DisplayAlert("Payment Failed", "Payment Failed", "OK");
                    await page.Navigation.PopAsync();
                }

                MessagingCenter.Unsubscribe<string, string>("PaymentRequest", "NotifyMsg");
            });

        }
        #endregion


        #region Model

        //bool isNotNullAdverts = false;
        //public bool IsNotNullAdverts
        //{
        //    get { return isNotNullAdverts; }
        //    set { SetProperty(ref isNotNullAdverts, value); }
        //}

        //bool isNullAdverts = false;
        //public bool IsNullAdverts
        //{
        //    get { return isNullAdverts; }
        //    set { SetProperty(ref isNullAdverts, value); }
        //}

        //string textColour = string.Empty;
        //public string TextColour
        //{
        //    get { return textColour; }
        //    set { SetProperty(ref textColour, value); }
        //}

        string textColour = string.Empty;
        public string TextColour
        {
            get { return textColour; }
            set
            {
                //SetProperty(ref isAdvert, value);
                textColour = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextColour"));
            }
        }

        string status = string.Empty;
        public string Status
        {
            get { return status; }
            set
            {
                //SetProperty(ref isAdvert, value);
                status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        //string status = string.Empty;
        //public string Status
        //{
        //    get { return status; }
        //    //set { SetProperty(ref status, value); }
        //}

        string category = string.Empty;
        public string Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }

        string phone = string.Empty;
        public string PhoneNumber
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        bool sendSms = false;
        public bool SendSms
        {
            get { return sendSms; }
            set { SetProperty(ref sendSms, value); }
        }

        string budget = string.Empty;
        public string Budget
        {
            get { return budget; }
            set { SetProperty(ref budget, value); }
        }

        bool isBusy = false;
        public new bool IsBusy
        {
            get { return isBusy; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        //bool isEnabled = false;
        //public bool IsEnabled
        //{
        //    get { return isEnabled; }
        //    set { SetProperty(ref isEnabled, value); }
        //}

        bool isEnabled = false;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
            }
        }

        //bool isDisabled = false;
        //public bool IsDisabled
        //{
        //    get { return isDisabled; }
        //    set { SetProperty(ref isDisabled, value); }
        //}

        bool isDisabled = false;
        public bool IsDisabled
        {
            get { return isDisabled; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isDisabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDisabled"));
            }
        }

        bool isNullAdverts = false;
        public bool IsNullAdverts
        {
            get { return isNullAdverts; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isNullAdverts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNullAdverts"));
            }
        }

        bool isNotNullAdverts = false;
        public bool IsNotNullAdverts
        {
            get { return isNotNullAdverts; }
            set
            {
                //SetProperty(ref isAdvert, value);
                isNotNullAdverts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNotNullAdverts"));
            }
        }

        //string amount = string.Empty;
        //public string Amount
        //{
        //    get { return amount; }
        //    set { SetProperty(ref amount, value); }
        //}

        

        string amount = string.Empty;
        public string Amount
        {
            get { return amount; }
            set
            {
                //SetProperty(ref isAdvert, value);
                amount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            }
        }
        #endregion

        #region load Promotion
        public void GetSearchPromotions(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetSearchCommand.Execute(mm);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            };

        }

        private Command getSearchCommand;

        public Command GetSearchCommand
        {
            get
            {
                return getSearchCommand ??
                    (getSearchCommand = new Command(async () => await ExecuteGetSearchCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetSearchCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetActionCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                ServiceList.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;

                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Supplier Services";
                if (itm.TransactionType == 0)
                {
                    trn.TransactionType = 1;
                }
                else
                {
                    trn.TransactionType = itm.TransactionType;
                }
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = "PROMOTION SEARCH";
                trn.AgentCode = itm.SupplierId;
                trn.Note = "PROMOTION SEARCH";
                trn.CustomerData = itm.Note;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                string img = itm.Image;
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    foreach (var it in servics)
                    {
                        if (it.MediaType.Trim() == "Image")
                        {
                            it.IsAdvert = true;
                            it.IsNotAdvert = false;
                            it.Media = "http://192.168.100.150:5000/Content//notify//notify.mp3";
                        }
                        else
                        {
                            it.IsAdvert = false;
                            it.IsNotAdvert = true;
                            it.Media = it.Image;
                            it.Image = "InsuranceBanner.png";
                        }
                        if (string.IsNullOrEmpty(it.UserImage))
                        {
                            it.UserImage = "http://192.168.100.150:5000/Content/Administration/images/user.png"; ;
                        }
                    }
                    SearchResults.ReplaceRange(servics);
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetServicesCommand.ChangeCanExecute();
            }

            //if (showAlert)
            //  await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region SwitchView
        public void SwitchViews()
        {
            ShowSearchList = true;
            hideSearchList = false;

        }
        #endregion

        #region load Actions
        public void GetServiceActions(MenuItem mm)
        {

            selectedService = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedService == null)
                return;

            if (ItemSelected == null)
            {

                GetActionCommand.Execute(null);
                SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(selectedService);
            }
        }

        private Command getActionCommand;

        public Command GetActionCommand
        {
            get
            {
                return getActionCommand ??
                    (getActionCommand = new Command(async () => await ExecuteGetActionCommand(selectedService), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetActionCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetActionCommand.ChangeCanExecute();
            var showAlert = false;
            if (itm.Section == "Loyalty")
            {
                #region loyalty
                if (itm.HasProducts)
                {
                    itm.Description = itm.Title;
                    await page.Navigation.PushAsync(new LoyaltyActions(itm));
                }
                else
                {
                    itm.Description = itm.Title;
                    await page.Navigation.PushAsync(new JoinLoyalty(itm));
                }
                #endregion
            }
            else
            {
                #region services
                try
                {

                    ServiceOptions.Clear();
                    List<MenuItem> mnu = new List<MenuItem>();
                    TransactionRequest trn = new TransactionRequest();
                    AccessSettings acnt = new Services.AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;
                    trn.CustomerAccount = uname + ":" + pass;
                    //trn.CustomerAccount = "263774090142:22398";
                    trn.MTI = "0300";
                    trn.ProcessingCode = "420000";
                    trn.Narrative = "Service Actions";
                    if (itm.TransactionType == 0)
                    {
                        trn.TransactionType = 1;
                    }
                    else
                    {
                        trn.TransactionType = itm.TransactionType;
                    }
                    trn.ServiceId = itm.ServiceId;
                    trn.ServiceProvider = itm.Section;
                    trn.AgentCode = itm.SupplierId;
                    trn.Product = itm.Id;
                    string Body = "";
                    Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    Body += "&AgentCode=" + trn.AgentCode;
                    Body += "&Action=Mobile";
                    Body += "&TerminalId=" + trn.TerminalId;
                    Body += "&TransactionRef=" + trn.TransactionRef;
                    Body += "&ServiceId=" + trn.ServiceId;
                    Body += "&Product=" + trn.Product;
                    Body += "&Amount=" + trn.Amount;
                    Body += "&MTI=" + trn.MTI;
                    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    Body += "&Narrative=" + trn.Narrative;
                    Body += "&CustomerData=" + trn.CustomerData;
                    Body += "&Quantity=" + trn.Quantity;
                    Body += "&Note=" + trn.Note;
                    Body += "&Mpin=" + trn.Mpin;
                    Body += "&TransactionType=" + trn.TransactionType;

                    HttpClient client = new HttpClient();
                    var myContent = Body;
                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);
                    if (result != "System.IO.MemoryStream")
                    {
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        foreach (var serv in servics)
                        {
                            if (serv.Image != null)
                            {
                                char[] delimiters = new char[] { '~' };
                                string[] supid = serv.Image.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                                serv.Image = HostDomain + supid[0];
                            }
                            else if (itm.Image != null)
                            {
                                serv.Image = itm.Image;
                            }
                        }
                        ServiceOptions.ReplaceRange(servics);
                    }

                }
                catch (Exception ex)
                {
                    IsBusy = true;
                    showAlert = true;

                }
                finally
                {
                    IsBusy = false;
                    GetServicesCommand.ChangeCanExecute();
                }
                #endregion
            }
            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region Render Actions
        public void RenderServiceAction(MenuItem mm)
        {

            renderService = mm;
            OnPropertyChanged("SelectedAction");
            if (renderService == null)
                return;

            if (ItemSelected == null)
            {

                RenderActionCommand.Execute(null);
                //   SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(renderService);
            }
        }

        private Command renderActionCommand;

        public Command RenderActionCommand
        {
            get
            {
                return renderActionCommand ??
                    (renderActionCommand = new Command(async () => await ExecuteRenderActionCommand(renderService), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteRenderActionCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            RenderActionCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                trn.Narrative = "Service";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Product = itm.Id;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var px = new MenuItem();
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    px = servics.FirstOrDefault();
                    if (response.ResponseCode == "00000")
                    {
                        switch (px.TransactionType)
                        {
                            case 9: // webview  
                                await page.Navigation.PushAsync(new WebviewPage(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                                break;
                            case 3:// "Payment":
                                await page.Navigation.PushAsync(new ServicePayment(px));
                                break;
                            case 13: // OTP        
                                await page.Navigation.PushAsync(new OTPPage(px));
                                break;
                            case 14: // file upload       
                                await page.Navigation.PushAsync(new FileUploadPage(px));
                                break;
                            case 15: // Signature 
                                await page.Navigation.PushAsync(new SignaturePage(px));
                                break;
                            case 16: // Signature 
                                await page.Navigation.PushAsync(new SharePage(px));
                                break;
                        }

                    }
                    else
                    {
                        await page.DisplayAlert("Transaction Error ", "Please try again letter ", "OK");
                    }
                }

            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                RenderActionCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Service Error", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region Get Leads
        public void GetLeads(MenuItem mm)
        {


            renderService = mm;
            OnPropertyChanged("SelectedAction");
            if (renderService == null)
                return;

            if (ItemSelected == null)
            {

                GetLeadsCommand.Execute(renderService);
                //   SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(renderService);
            }
        }

        private Command getLeadsCommand;

        public Command GetLeadsCommand
        {
            get
            {
                return getLeadsCommand ??
                    (getLeadsCommand = new Command(async () => await ExecuteGetLeadsCommand(renderService), () => { return !IsBusy; }));
            }
        }

        AccessSettings ac = new Services.AccessSettings();

        private async Task ExecuteGetLeadsCommand(MenuItem menuItem)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetLeadsCommand.ChangeCanExecute();
            var showAlert = false;

            #region get location 
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude.ToString();
                    Longitude = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
            }
            catch
            {
                try
                {
                    AccessSettings ac = new Services.AccessSettings();

                    bool shownotify = false;
                    var sh = ac.GetSetting("LocationNotification").Result;

                    if (sh == null) shownotify = true;
                    if (sh != null)
                    {
                        var e = DateTime.Parse(sh.ToString());
                        if (DateTime.Now.Subtract(e).Minutes > 30)
                        {
                            shownotify = true;
                        }

                    }
                    if (shownotify)
                    {
                        var notificator = DependencyService.Get<IToastNotificator>();

                        var options = new NotificationOptions()
                        {
                            IsClickable = true,
                            Title = "Switch on your location",
                            Description = "To get deals near you on YoApp, switch on your location!"
                        };

                        var result = notificator.Notify(options);

                        var resp = ac.SaveValue("LocationNotification", DateTime.Now.ToString()).Result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Latitude = location.Latitude.ToString();
                        Longitude = location.Longitude.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            try
            {
                ServiceOptions.Clear();
                
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                // AccessSettings acnt = new Services.AccessSettings();

                #region Build Chat Id
                chatMessage = new ChatMessage();
                chatMessage.ReceiverId = menuItem.SupplierId.Trim();
                chatMessage.ReceiverName = menuItem.Note;
                chatMessage.AgentId = Convert.ToInt64(menuItem.Id);
                chatMessage.BidId = 0;
                chatMessage.JobId = 0;

                var Id = chatMessage.ReceiverId + "_" + chatMessage.ReceiverName + "_" + chatMessage.AgentId + "_" + "Supplier" + "_" + chatMessage.BidId + "_" + chatMessage.JobId;
                #endregion

                string pass = ac.Password;
                string uname = ac.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "460000";
                trn.Narrative = "Home";
                trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "LEAD";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = 0;// redeemSection.ServiceId;
                trn.Amount = 0;
                trn.CustomerData = menuItem.SupplierId + "," + menuItem.Id; // ServiceProvider,PromotionId
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&Note=" + trn.Note;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var px = new MenuItem();
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    px = servics.FirstOrDefault();

                    if (response.ResponseCode == "00000")
                    {

                        await page.Navigation.PushAsync(new SupplierChat(Id)); // open supplier chat
                    }
                    else
                    {
                        await page.DisplayAlert("Error!", "There has been an error in connecting you with supplier", "Ok");

                        await page.Navigation.PushAsync(new SupplierChat(Id)); // open supplier chat
                    }
                }
                else
                {
                    await page.DisplayAlert("Error!", "There has been an error in connecting you with supplier", "Ok");

                    await page.Navigation.PushAsync(new SupplierChat(Id)); // open supplier chat
                }
            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = false;

                await page.DisplayAlert("Error!", "There has been an error in connecting you with supplier", "Ok");

                // await page.Navigation.PushAsync(new SupplierChat(Id)); // open supplier chat
            }
            finally
            {
                IsBusy = false;
                RenderActionCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Service Error", "Unable to gather " + menuItem.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region Disable Ad
        public void DisableAd(MenuItem mm)
        {
            renderService = mm;
            OnPropertyChanged("SelectedAction");
            if (renderService == null)
                return;

            if (ItemSelected == null)
            {

                DisableAdCommand.Execute(renderService);
                //   SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(renderService);
            }
        }

        private Command disableAdCommand;

        public Command DisableAdCommand
        {
            get
            {
                return disableAdCommand ??
                    (disableAdCommand = new Command(async () => await ExecuteDisableAdCommand(renderService), () => { return !IsBusy; }));
            }
        }

        //AccessSettings access = new Services.AccessSettings();

        private async Task ExecuteDisableAdCommand(MenuItem menuItem)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

            DisableAdCommand.ChangeCanExecute();
            var showAlert = false;

            #region get location 
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude.ToString();
                    Longitude = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
            }
            catch
            {
                try
                {
                    AccessSettings ac = new Services.AccessSettings();

                    bool shownotify = false;
                    var sh = ac.GetSetting("LocationNotification").Result;

                    if (sh == null) shownotify = true;
                    if (sh != null)
                    {
                        var e = DateTime.Parse(sh.ToString());
                        if (DateTime.Now.Subtract(e).Minutes > 30)
                        {
                            shownotify = true;
                        }

                    }
                    if (shownotify)
                    {
                        var notificator = DependencyService.Get<IToastNotificator>();

                        var options = new NotificationOptions()
                        {
                            IsClickable = true,
                            Title = "Switch on your location",
                            Description = "To get deals near you on YoApp, switch on your location!"
                        };

                        var result = notificator.Notify(options);

                        var resp = ac.SaveValue("LocationNotification", DateTime.Now.ToString()).Result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Latitude = location.Latitude.ToString();
                        Longitude = location.Longitude.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            var disableResult = await page.DisplayAlert("Alert!", "Do you really want to disable this Ad?", "Yes", "No");

            if (disableResult)
            {
                IsBusy = true;
                Message = "Disabling Ad...";

                try
                {
                    ServiceOptions.Clear();

                    List<MenuItem> mnu = new List<MenuItem>();
                    TransactionRequest trn = new TransactionRequest();
                    // AccessSettings acnt = new Services.AccessSettings();

                    #region Build Chat Id
                    chatMessage = new ChatMessage();
                    chatMessage.ReceiverId = menuItem.SupplierId.Trim();
                    chatMessage.ReceiverName = menuItem.Note;
                    chatMessage.AgentId = Convert.ToInt64(menuItem.Id);
                    chatMessage.BidId = 0;
                    chatMessage.JobId = 0;

                    var Id = chatMessage.ReceiverId + "_" + chatMessage.ReceiverName + "_" + chatMessage.AgentId + "_" + "Supplier" + "_" + chatMessage.BidId + "_" + chatMessage.JobId;
                    #endregion

                    string pass = ac.Password;
                    string uname = ac.UserName;
                    trn.CustomerAccount = uname + ":" + pass;
                    //trn.CustomerAccount = "263774090142:22398";
                    trn.Product = menuItem.Id;
                    trn.MTI = "0300";
                    trn.ProcessingCode = "460000";
                    trn.Narrative = "Home";
                    trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                    trn.AgentCode = "0";
                    trn.Quantity = 0;//redeemSection.Count;
                    trn.ServiceProvider = "DISABLE AD";
                    trn.TransactionRef = "Mobile";
                    trn.ServiceId = 0;// redeemSection.ServiceId;
                    trn.Amount = 0;
                    trn.CustomerData = menuItem.SupplierId + "," + menuItem.Id; // ServiceProvider,PromotionId
                    string Body = "";
                    Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    Body += "&AgentCode=" + trn.AgentCode;
                    Body += "&Action=Mobile";
                    Body += "&TerminalId=" + trn.TerminalId;
                    Body += "&TransactionRef=" + trn.TransactionRef;
                    Body += "&ServiceId=" + trn.ServiceId;
                    Body += "&Product=" + trn.Product;
                    Body += "&Amount=" + trn.Amount;
                    Body += "&MTI=" + trn.MTI;
                    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    Body += "&Narrative=" + trn.Narrative;
                    Body += "&CustomerData=" + trn.CustomerData;
                    Body += "&Quantity=" + trn.Quantity;
                    Body += "&Mpin=" + trn.Mpin;
                    Body += "&Note=" + trn.Note;

                    HttpClient client = new HttpClient();

                    var myContent = Body;

                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);

                    if (result != "System.IO.MemoryStream")
                    {
                        //var px = new MenuItem();
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        //var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                       // px = servics.FirstOrDefault();

                        if (response.ResponseCode == "00000")
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Deactivation Successful!", "Your Ad was suceesfully deactivated", "Ok");

                            MenuItem mn = new YomoneyApp.MenuItem();
                            mn.Title = "My Promotions";
                            mn.TransactionType = 23;
                            mn.Section = "PROMOTIONS";
                            mn.SupplierId = "All";

                            await page.Navigation.PushAsync(new MyPromotions(mn));
                        }
                        else
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Deactivation Error!", "There has been an error in disabling your ad. Please check your internet connection and try again", "Ok");
                        }
                    }
                    else
                    {
                        IsBusy = false;

                        await page.DisplayAlert("Ad Deactivation Error!", "There has been an error in disabling your ad. Please check your internet connection and try again", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    // showAlert = false;

                    await page.DisplayAlert("Ad Deactivation Error!", "There has been an error in disabling your ad. Please check your internet connection and try again", "Ok");
                }
                finally
                {
                    IsBusy = false;
                    DisableAdCommand.ChangeCanExecute();
                }

                //if (showAlert)
                //    await page.DisplayAlert("Service Error", "Unable to gather " + menuItem.Title + ". Check your internet connection", "OK");

            }
            else
            {
                IsBusy = false;

                MenuItem mn = new YomoneyApp.MenuItem();
                mn.Title = "My Promotions";
                mn.TransactionType = 23;
                mn.Section = "PROMOTIONS";
                mn.SupplierId = "All";

                await page.Navigation.PushAsync(new MyPromotions(mn));
            }            
        }
        #endregion

        #region Delete Ad
        public void DeleteAd(MenuItem mm)
        {
            renderService = mm;
            OnPropertyChanged("SelectedAction");
            if (renderService == null)
                return;

            if (ItemSelected == null)
            {

                DeleteAdCommand.Execute(renderService);
                //   SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(renderService);
            }
        }

        private Command deleteAdCommand;

        public Command DeleteAdCommand
        {
            get
            {
                return deleteAdCommand ??
                    (deleteAdCommand = new Command(async () => await ExecuteDeleteAdCommand(renderService), () => { return !IsBusy; }));
            }
        }

        //AccessSettings access = new Services.AccessSettings();

        private async Task ExecuteDeleteAdCommand(MenuItem menuItem)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

            DisableAdCommand.ChangeCanExecute();
            var showAlert = false;

            #region get location 
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude.ToString();
                    Longitude = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
            }
            catch
            {
                try
                {
                    AccessSettings ac = new Services.AccessSettings();

                    bool shownotify = false;
                    var sh = ac.GetSetting("LocationNotification").Result;

                    if (sh == null) shownotify = true;
                    if (sh != null)
                    {
                        var e = DateTime.Parse(sh.ToString());
                        if (DateTime.Now.Subtract(e).Minutes > 30)
                        {
                            shownotify = true;
                        }

                    }
                    if (shownotify)
                    {
                        var notificator = DependencyService.Get<IToastNotificator>();

                        var options = new NotificationOptions()
                        {
                            IsClickable = true,
                            Title = "Switch on your location",
                            Description = "To get deals near you on YoApp, switch on your location!"
                        };

                        var result = notificator.Notify(options);

                        var resp = ac.SaveValue("LocationNotification", DateTime.Now.ToString()).Result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Latitude = location.Latitude.ToString();
                        Longitude = location.Longitude.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            var deleteResult = await page.DisplayAlert("Alert!", "Do you really want to delete this Ad?", "Yes", "No");

            if (deleteResult)
            {
                IsBusy = true;
                Message = "Deleting Ad...";

                try
                {
                    ServiceOptions.Clear();

                    List<MenuItem> mnu = new List<MenuItem>();
                    TransactionRequest trn = new TransactionRequest();
                    // AccessSettings acnt = new Services.AccessSettings();

                    #region Build Chat Id
                    chatMessage = new ChatMessage();
                    chatMessage.ReceiverId = menuItem.SupplierId.Trim();
                    chatMessage.ReceiverName = menuItem.Note;
                    chatMessage.AgentId = Convert.ToInt64(menuItem.Id);
                    chatMessage.BidId = 0;
                    chatMessage.JobId = 0;

                    var Id = chatMessage.ReceiverId + "_" + chatMessage.ReceiverName + "_" + chatMessage.AgentId + "_" + "Supplier" + "_" + chatMessage.BidId + "_" + chatMessage.JobId;
                    #endregion

                    string pass = ac.Password;
                    string uname = ac.UserName;
                    trn.CustomerAccount = uname + ":" + pass;
                    //trn.CustomerAccount = "263774090142:22398";
                    trn.Product = menuItem.Id;
                    trn.MTI = "0300";
                    trn.ProcessingCode = "460000";
                    trn.Narrative = "Home";
                    trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                    trn.AgentCode = "0";
                    trn.Quantity = 0;//redeemSection.Count;
                    trn.ServiceProvider = "DELETE AD";
                    trn.TransactionRef = "Mobile";
                    trn.ServiceId = 0;// redeemSection.ServiceId;
                    trn.Amount = 0;
                    trn.CustomerData = menuItem.SupplierId + "," + menuItem.Id; // ServiceProvider,PromotionId
                    string Body = "";
                    Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    Body += "&AgentCode=" + trn.AgentCode;
                    Body += "&Action=Mobile";
                    Body += "&TerminalId=" + trn.TerminalId;
                    Body += "&TransactionRef=" + trn.TransactionRef;
                    Body += "&ServiceId=" + trn.ServiceId;
                    Body += "&Product=" + trn.Product;
                    Body += "&Amount=" + trn.Amount;
                    Body += "&MTI=" + trn.MTI;
                    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    Body += "&Narrative=" + trn.Narrative;
                    Body += "&CustomerData=" + trn.CustomerData;
                    Body += "&Quantity=" + trn.Quantity;
                    Body += "&Mpin=" + trn.Mpin;
                    Body += "&Note=" + trn.Note;

                    HttpClient client = new HttpClient();

                    var myContent = Body;

                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);

                    if (result != "System.IO.MemoryStream")
                    {
                       // var px = new MenuItem();
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        //var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                       // px = servics.FirstOrDefault();

                        if (response.ResponseCode == "00000")
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Deletion Successful!", "Your Ad was suceesfully deleted", "Ok");

                            MenuItem mn = new YomoneyApp.MenuItem();
                            mn.Title = "My Promotions";
                            mn.TransactionType = 23;
                            mn.Section = "PROMOTIONS";
                            mn.SupplierId = "All";

                            await page.Navigation.PushAsync(new MyPromotions(mn));
                        }
                        else
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Deletion Error!", "There has been an error in deleting your ad. Please check your internet connection and try again", "Ok");
                        }
                    }
                    else
                    {
                        IsBusy = false;

                        await page.DisplayAlert("Ad Deletion Error!", "There has been an error in deleting your ad. Please check your internet connection and try again", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    // showAlert = false;

                    await page.DisplayAlert("Ad Deletion Error!", "There has been an error in deleting your ad. Please check your internet connection and try again", "Ok");
                }
                finally
                {
                    IsBusy = false;
                    DisableAdCommand.ChangeCanExecute();
                }

                //if (showAlert)
                //    await page.DisplayAlert("Service Error", "Unable to gather " + menuItem.Title + ". Check your internet connection", "OK");

            }
            else
            {
                MenuItem mn = new YomoneyApp.MenuItem();
                mn.Title = "My Promotions";
                mn.TransactionType = 23;
                mn.Section = "PROMOTIONS";
                mn.SupplierId = "All";

                await page.Navigation.PushAsync(new MyPromotions(mn));
            }
        }
        #endregion

        #region Delete Ad
        public void EnableAd(MenuItem mm)
        {
            renderService = mm;
            OnPropertyChanged("SelectedAction");
            if (renderService == null)
                return;

            if (ItemSelected == null)
            {

                EnableAdCommand.Execute(renderService);
                //   SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(renderService);
            }
        }

        private Command enableAdCommand;

        public Command EnableAdCommand
        {
            get
            {
                return enableAdCommand ??
                    (enableAdCommand = new Command(async () => await ExecuteEnableAdCommand(renderService), () => { return !IsBusy; }));
            }
        }

        //AccessSettings access = new Services.AccessSettings();

        private async Task ExecuteEnableAdCommand(MenuItem menuItem)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                DisableAdCommand.ChangeCanExecute();
            var showAlert = false;

            #region get location 
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude.ToString();
                    Longitude = location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
            }
            catch
            {
                try
                {
                    AccessSettings ac = new Services.AccessSettings();

                    bool shownotify = false;
                    var sh = ac.GetSetting("LocationNotification").Result;

                    if (sh == null) shownotify = true;
                    if (sh != null)
                    {
                        var e = DateTime.Parse(sh.ToString());
                        if (DateTime.Now.Subtract(e).Minutes > 30)
                        {
                            shownotify = true;
                        }

                    }
                    if (shownotify)
                    {
                        var notificator = DependencyService.Get<IToastNotificator>();

                        var options = new NotificationOptions()
                        {
                            IsClickable = true,
                            Title = "Switch on your location",
                            Description = "To get deals near you on YoApp, switch on your location!"
                        };

                        var result = notificator.Notify(options);

                        var resp = ac.SaveValue("LocationNotification", DateTime.Now.ToString()).Result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Latitude = location.Latitude.ToString();
                        Longitude = location.Longitude.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion

            var disableResult = await page.DisplayAlert("Alert!", "Do you really want to activate this Ad?", "Yes", "No");

            if (disableResult)
            {
                IsBusy = true;
                Message = "Enabling Ad...";

                try
                {
                    ServiceOptions.Clear();

                    List<MenuItem> mnu = new List<MenuItem>();
                    TransactionRequest trn = new TransactionRequest();
                    // AccessSettings acnt = new Services.AccessSettings();

                    #region Build Chat Id
                    chatMessage = new ChatMessage();
                    chatMessage.ReceiverId = menuItem.SupplierId.Trim();
                    chatMessage.ReceiverName = menuItem.Note;
                    chatMessage.AgentId = Convert.ToInt64(menuItem.Id);
                    chatMessage.BidId = 0;
                    chatMessage.JobId = 0;

                    var Id = chatMessage.ReceiverId + "_" + chatMessage.ReceiverName + "_" + chatMessage.AgentId + "_" + "Supplier" + "_" + chatMessage.BidId + "_" + chatMessage.JobId;
                    #endregion

                    string pass = ac.Password;
                    string uname = ac.UserName;
                    trn.CustomerAccount = uname + ":" + pass;
                    //trn.CustomerAccount = "263774090142:22398";
                    trn.Product = menuItem.Id;
                    trn.MTI = "0300";
                    trn.ProcessingCode = "460000";
                    trn.Narrative = "Home";
                    trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                    trn.AgentCode = "0";
                    trn.Quantity = 0;//redeemSection.Count;
                    trn.ServiceProvider = "ENABLE AD";
                    trn.TransactionRef = "Mobile";
                    trn.ServiceId = 0;// redeemSection.ServiceId;
                    trn.Amount = 0;
                    trn.CustomerData = menuItem.SupplierId + "," + menuItem.Id; // ServiceProvider,PromotionId
                    string Body = "";
                    Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    Body += "&AgentCode=" + trn.AgentCode;
                    Body += "&Action=Mobile";
                    Body += "&TerminalId=" + trn.TerminalId;
                    Body += "&TransactionRef=" + trn.TransactionRef;
                    Body += "&ServiceId=" + trn.ServiceId;
                    Body += "&Product=" + trn.Product;
                    Body += "&Amount=" + trn.Amount;
                    Body += "&MTI=" + trn.MTI;
                    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    Body += "&Narrative=" + trn.Narrative;
                    Body += "&CustomerData=" + trn.CustomerData;
                    Body += "&Quantity=" + trn.Quantity;
                    Body += "&Mpin=" + trn.Mpin;
                    Body += "&Note=" + trn.Note;

                    HttpClient client = new HttpClient();

                    var myContent = Body;

                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);

                    if (result != "System.IO.MemoryStream")
                    {
                        var px = new MenuItem();
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        px = servics.FirstOrDefault();

                        if (response.ResponseCode == "00000")
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Enabled Successfully!", "Your Ad was suceesfully enabled", "Ok");

                            MenuItem mn = new YomoneyApp.MenuItem();
                            mn.Title = "My Promotions";
                            mn.TransactionType = 23;
                            mn.Section = "PROMOTIONS";
                            mn.SupplierId = "All";

                            await page.Navigation.PushAsync(new MyPromotions(mn));
                        }
                        else
                        {
                            IsBusy = false;

                            await page.DisplayAlert("Ad Enabled Error!", "There has been an error in enabling your ad. Please check your internet connection and try again", "Ok");
                        }
                    }
                    else
                    {
                        IsBusy = false;

                        await page.DisplayAlert("Ad Enabling Error!", "There has been an error in enabling your ad. Please check your internet connection and try again", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    // showAlert = false;

                    await page.DisplayAlert("Ad Enabled Error!", "There has been an error in disabling your ad. Please check your internet connection and try again", "Ok");
                }
                finally
                {
                    IsBusy = false;
                    EnableAdCommand.ChangeCanExecute();
                }

                //if (showAlert)
                //    await page.DisplayAlert("Service Error", "Unable to gather " + menuItem.Title + ". Check your internet connection", "OK");

            }
            else
            {
                MenuItem mn = new YomoneyApp.MenuItem();
                mn.Title = "My Promotions";
                mn.TransactionType = 23;
                mn.Section = "PROMOTIONS";
                mn.SupplierId = "All";

                await page.Navigation.PushAsync(new MyPromotions(mn));
            }
        }
        #endregion

        #region cancel 
        private Command getCancelCommand;

        public Command GetCancelCommand
        {
            get
            {
                return getCancelCommand ??
                    (getCancelCommand = new Command(async () => await ExecuteGetCancelCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetCancelCommand()
        {
            //await page.Navigation.PopModalAsync();
            await App.Current.MainPage.Navigation.PopModalAsync();
            // await page.Navigation.PopAsync();
        }
        #endregion
    }

}

