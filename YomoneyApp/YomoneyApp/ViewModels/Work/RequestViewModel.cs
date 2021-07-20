using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MvvmHelpers;
using YomoneyApp.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using RetailKing.Models;
using YomoneyApp.Views.Spani;
using static System.DateTime;
using YomoneyApp.Services;
using YomoneyApp.Views.TemplatePages;
using YomoneyApp.Views.Chat;
using YomoneyApp.Views.Services;
using Xamarin.Essentials;
using YomoneyApp.Models;

namespace YomoneyApp
{
    public class RequestViewModel : ViewModelBase 
    {
        string HostDomain = "http://192.168.100.150:5000";
        string ProcessingCode = "350000";
     
        readonly IDataStore dataStore;
        public bool ForceSync { get; set; }
        public ObservableRangeCollection<MenuItem> Stores { get; set; }
        public ObservableRangeCollection<MenuItem> Categories { get; set; }
        public ObservableRangeCollection<Location> Geolocations { get; set; }
        public ObservableRangeCollection<Placemark> GeoPlaces { get; set; }
        public ObservableCollection<string> SubCategories { get; set; }
        public ObservableRangeCollection<MenuItem> Currencies { get; set; }
        public RequestViewModel(Page page) : base(page)
        {
            dataStore = DependencyService.Get<IDataStore>();
            Stores = new ObservableRangeCollection<MenuItem>();
            Categories = new ObservableRangeCollection<MenuItem>();
            Geolocations = new ObservableRangeCollection<Location>();
            GeoPlaces = new ObservableRangeCollection<Placemark>();
            SubCategories = new ObservableCollection<string>();
            Currencies = new ObservableRangeCollection<MenuItem>();
        }
        public Action<MenuItem> ItemSelected { get; set; }

        MenuItem  selectedJob;

        public MenuItem SelectedJob
        {
            get { return selectedJob; }
            set
            {
                selectedJob = value;
                OnPropertyChanged("SelectedJob");
                if (selectedJob == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                   // page.Navigation.PushModalAsync(new RequestBids(SelectedJob));
                    SelectedJob = null;
                    selectedJob = null;
                    //page.Navigation.PopAsync();
                }
                else
                {
                    ItemSelected.Invoke(selectedJob);
                }
            }
        }


        MenuItem cancelJob;

        public MenuItem CancelJob
        {
            get { return cancelJob; }
            set
            {
                cancelJob = value;
                OnPropertyChanged("SelectedJob");
                if (cancelJob == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                    // page.Navigation.PushModalAsync(new RequestBids(SelectedJob));
                    CancelJob = null;
                    cancelJob = null;
                    //page.Navigation.PopAsync();
                }
                else
                {
                    ItemSelected.Invoke(cancelJob);
                }
            }
        }

        MenuItem myRequestQuotes;

        public MenuItem MyRequestQuotes
        {
            get { return myRequestQuotes; }
            set
            {
                myRequestQuotes = value;
                OnPropertyChanged("MyRequestQuotes");
                if (myRequestQuotes == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                    page.Navigation.PushAsync(new JobRequest());
                    MyRequestQuotes = null;
                    myRequestQuotes = null;
                }
                else
                {
                    ItemSelected.Invoke(myRequestQuotes);
                }
            }
        }

        MenuItem myQuote;

        public MenuItem MyQuote
        {
            get { return myQuote; }
            set
            {
                myQuote = value;
                OnPropertyChanged("MyQuote");
                if (myQuote == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                   // page.Navigation.PushModalAsync(new QuoteDetails(myQuote));
                    myQuote = null;
                    MyQuote = null;
                    // page.Navigation.PopAsync();
                }
                else
                {
                    ItemSelected.Invoke(myQuote);
                }
            }
        }

        INavigation currentPage;

        public INavigation CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
                if (currentPage == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                    page.Navigation.PushAsync(new QuoteDetails(myQuote));
                    myQuote = null;
                    MyQuote = null;
                }
                else
                {
                    //CurrentPage.Invoke(currentPage);
                }
            }
        }

        MenuItem placeBid;

        public MenuItem PlaceBid
        {
            get { return placeBid; }
            set
            {
                placeBid = value;
                OnPropertyChanged("PlaceBid");
                if (placeBid == null)
                    return;

                if (ItemSelected == null)
                {         
                   // page.Navigation.PushModalAsync(new PlaceBid(placeBid));
                    placeBid = null;
                    PlaceBid = null;
                    // page.Navigation.PopAsync();
                }
                else
                {
                    ItemSelected.Invoke(placeBid);
                }
            }
        }

        MenuItem postSection;

        public MenuItem PostSection
        {
            get { return postSection; }
            set
            {
                postSection = value;
                OnPropertyChanged("SelectedLoyalty");
                if (postSection == null)
                    return;

                if (ItemSelected == null)
                {
                    RewardAlerts();
                    PostSection = null;
                    postSection = null;
                    // RedeemSection = null;  
                }
                else
                {
                    ItemSelected.Invoke(postSection);
                }
            }
        }

        private Command forceRefreshCommand;

        public Command ForceRefreshCommand
        {
            get
            {
                return forceRefreshCommand ??
                    (forceRefreshCommand = new Command(async () =>
                    {
                        ForceSync = true;
                        await ExecuteGetStoresCommand();
                    }));
            }
        }

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
            await page.Navigation.PopAsync();
        }
        #endregion

        #region Geo Address
        Location selectedAdd;

        public  Location SelectedAdd
        {
            get { return selectedAdd; }
            set
            {
                selectedAdd = value;
                OnPropertyChanged("SelectedJob");
                if (selectedJob == null)
                    return;

                if (ItemSelected == null)
                {
                    //award bid
                    // page.Navigation.PushModalAsync(new RequestBids(SelectedJob));
                   
                    Longitude  = selectedAdd.Longitude;
                    Latitude  = selectedAdd.Latitude;
                    GetGeoPlaceAsync(Latitude, Longitude);
                    
                    //page.Navigation.PopAsync();
                }
                else
                {
                    ItemSelected.Invoke(selectedJob);
                }
            }
        }
        public async Task<IEnumerable<Location>> GetGeoAddressAsync(string address)
        {
            if (IsBusy)
             return new List<Location>();

            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetStoresCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                AccessSettings acnt = new Services.AccessSettings();

                //string pass = acnt.Password;
                string uname = acnt.UserName;
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = uname.Substring(uname.Length - 6, 6) + DateTime.Now.ToString("HHmm");

                }
                var client = new HttpClient();
                string APIKey = "AIzaSyDWd6uSjjohKOWKMa5tefGw30Uk3CkbeJ0";
                string paramlocal = string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/json?input={1}&fields=address,geocode&key={0}&ssiontoken={2}", APIKey, address, uname);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<RootObject>(result);
                    
                    //GeoPlaces.Clear();
                    List<Placemark> locs = new List<Placemark>();
                    foreach (var item in response.predictions)
                    {
                        Placemark suggest = new Placemark();
                        suggest.SubLocality = item.description;
                        locs.Add(suggest);
                    }
                    GeoPlaces.ReplaceRange(locs);

                }
                var locations = await Geocoding.GetLocationsAsync(address);
                return locations;
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetStoresCommand.ChangeCanExecute();
            }

            return new List<Location>();

        }

        public async void GetGeoPlaceAsync(double lat, double lon)
        {
           
                IsBusy = true;
            
            var showAlert = false;
            try
            {
                var locations = await Geocoding.GetPlacemarksAsync (lat,lon);
                var Addr = locations.FirstOrDefault();
                if (!string.IsNullOrEmpty(Addr.SubLocality))
                {
                    Address = Addr.SubLocality;
                }
               
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetStoresCommand.ChangeCanExecute();
            }

           
        }
        #endregion

        #region Categories
        private Command getStoresCommand;

        public Command GetStoresCommand
        {
            get
            {
                return getStoresCommand ??
                    (getStoresCommand = new Command(async () => await ExecuteGetStoresCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetStoresCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetStoresCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();

                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "7";
                trn.Note = "JobSubCategory";
                trn.Product = "";
               // trn.AgentCode = selectedOption.SupplierId;
               // trn.Quantity = selectedOption.Count;
               // trn.Product = selectedOption.Description;
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

                var client = new HttpClient(); 
               // var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", Body);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Stores.ReplaceRange(servics);

                }
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetStoresCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather job categories.", "OK");


        }
        #endregion

        #region Subcategories
        private Command getSubcategoriesCommand;

        public Command GetSubcategoriesCommand
        {
            get
            {
                return getSubcategoriesCommand ??
                    (getSubcategoriesCommand = new Command(async () => await ExecuteGetSubcategoriesCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetSubcategoriesCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetStoresCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "7";
                trn.Note = "JobSubCategory";
                trn.Product = Category;
                // trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    var ar = servics.Select(e => e.Title.Trim()).ToArray();
                    foreach (var store in ar)
                        SubCategories.Add(store);
                   
                }
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetSubcategoriesCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather "+ Category + " services.", "OK");


        }

        public async Task<IEnumerable<MenuItem>> GetStoreAsync( string note)
        {
            if (IsBusy)
                return new List<MenuItem>();

            IsBusy = true;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "7";
                trn.Note = note;// "JobSectors";
                trn.Product = Category;
                // trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;

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
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather services.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }
        #endregion

        #region Currencies
        private Command getCurenciesCommand;

        public Command GetCurrenciesCommand
        {
            get
            {
                return getCurenciesCommand ??
                    (getCurenciesCommand = new Command(async () => await ExecuteGetCurrenciesCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetCurrenciesCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetCurrenciesCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "470000";
                trn.Narrative = "7";
                trn.Note = "Currencies";
                trn.Product = Category;
                // trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Currencies.ReplaceRange(servics);
                }
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetSubcategoriesCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + Category + " services.", "OK");


        }

        public async Task<IEnumerable<MenuItem>> GetCurrenciesAsync()
        {
            if (IsBusy)
                return new List<MenuItem>();

            IsBusy = true;
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "470000";
                trn.Narrative = "7";
                trn.Note = "Currencies";
                trn.Product = Category;
                // trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Currencies.ReplaceRange(servics);
                    return servics;

                }
                return new List<MenuItem>();

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather services.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }
        #endregion

        #region My Job Requests
        private Command jobsRefreshCommand;

        public Command JobsRefreshCommand
        {
            get
            {
                return jobsRefreshCommand ??
                    (jobsRefreshCommand = new Command(async () =>
                    {
                        ForceSync = true;
                        PageNumber += 1;
                        IsBusy = false;
                        await ExecuteJobListCommand();
                    }));
            }
        }

        private Command getJobListCommand;

        public Command GetJobListCommand
        {
            get
            {
                return getJobListCommand ??
                    (getJobListCommand = new Command(async () => await ExecuteJobListCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteJobListCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetJobListCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
               // if (ServiceOptions != null)
                 //   ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "7";
                trn.Note = "Requests";
                /*trn.AgentCode = selectedOption.SupplierId;
                trn.Quantity = selectedOption.Count;
                trn.Product = selectedOption.Description;*/
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }

                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }

                            }
                            else if (diff.TotalHours >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) * (int)(diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }

                            }
                            itm.SupplierId = "View Quotes";
                        }
                        servics.OrderByDescending(u => u.date).ToList();
                        Stores.ReplaceRange(servics);
                    }
                    else
                    {
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "You have no active service requests";
                        mn.Image = "http://192.168.100.150:5000/Content/Spani/Images/Oppotunity.jpg";
                        mn.HasProducts = false; // use it as show navigation
                        mn.IsEmptyList = true;
                        List<MenuItem> resp = new List<MenuItem>();
                        resp.Add(mn);
                        Stores.ReplaceRange(resp);
                        //await page.Navigation.PushModalAsync(new EmptyList(mn));
                        //await page.Navigation.PopAsync();
                    }
                    

                }

            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetJobListCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather You Job requests.", "OK");
        }

        private Command cancelJobRequestCommand;

        public Command CancelJobRequestCommand
        {
            get
            {
                return cancelJobRequestCommand ??
                    (cancelJobRequestCommand = new Command(async () => await ExecuteCancelJobRequestCommand(cancelJob), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteCancelJobRequestCommand(MenuItem mn)
        {
            if (IsBusy)
                return;

            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetJobListCommand.ChangeCanExecute();
            var showAlert = false;
            var answer = await page.DisplayAlert("Confirm", "Are you sure you want to remove this job request?", "Yes", "No");
            if (answer == true)
            {
                try
                {
                    // if (ServiceOptions != null)
                    //   ServiceOptions.Clear();
                    List<MenuItem> mnu = new List<MenuItem>();
                    TransactionRequest trn = new TransactionRequest();
                    AccessSettings acnt = new Services.AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;

                    trn.CustomerAccount = uname + ":" + pass;
                    trn.MTI = "0400";
                    trn.ProcessingCode = "510000";
                    JobPost jp = new JobPost();
                    jp.Id = long.Parse(mn.Id);
                    trn.Narrative = JsonConvert.SerializeObject(jp);
                    trn.Note = "Requests";
                    /*trn.AgentCode = selectedOption.SupplierId;
                    trn.Quantity = selectedOption.Count;
                    trn.Product = selectedOption.Description;*/
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

                    HttpClient client = new HttpClient();
                    var myContent = Body;
                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);
                    if (result != "System.IO.MemoryStream")
                    {
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        if (servics.Count > 0)
                        {
                            foreach (var itm in servics)
                            {
                                var diff = DateTime.Now.Subtract(itm.date);
                                if (diff.TotalDays > 30)
                                {
                                    var days = 0;
                                    if (diff.Days < 0)
                                    { days = (-1) * (diff.Days); }
                                    else { days = diff.Days; }
                                    if (string.IsNullOrEmpty(itm.Note))
                                    {
                                        itm.Note = itm.date.ToString(" dd MMM yyyy");
                                    }
                                    else
                                    {
                                        itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                    }

                                }
                                else if (diff.TotalDays >= 1)
                                {
                                    var days = 0;
                                    if (diff.TotalDays < 0)
                                    { days = (-1) * (int)(diff.TotalDays); }
                                    else { days = (int)diff.TotalDays; }
                                    if (string.IsNullOrEmpty(itm.Note))
                                    {
                                        itm.Note = days + " days ago";
                                    }
                                    else
                                    {
                                        itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                    }

                                }
                                else if (diff.TotalHours >= 1)
                                {
                                    var Hours = 0;
                                    if (diff.TotalHours < 0)
                                    { Hours = (-1) * (int)(diff.TotalHours); }
                                    else { Hours = (int)diff.TotalHours; }
                                    if (string.IsNullOrEmpty(itm.Note))
                                    {
                                        itm.Note = Hours + " hrs ago";
                                    }
                                    else
                                    {
                                        itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                    }
                                }
                                else
                                {
                                    var Minutes = 0;
                                    if (diff.TotalMinutes < 0)
                                    { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                    else { Minutes = (int)diff.TotalMinutes; }
                                    if (string.IsNullOrEmpty(itm.Note))
                                    {
                                        itm.Note = Minutes + " mins ago";
                                    }
                                    else
                                    {
                                        itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                    }

                                }
                                itm.SupplierId = "View Quotes";
                            }
                            Stores.ReplaceRange(servics);
                        }
                        else
                        {
                            mn = new YomoneyApp.MenuItem();
                            mn.Description = "You have no active service requests";
                            mn.Image = "http://192.168.100.150:5000/Content/Spani/Images/Oppotunity.jpg";
                            mn.HasProducts = false; // use it as show navigation
                            mn.IsEmptyList = true;
                            List<MenuItem> resp = new List<MenuItem>();
                            resp.Add(mn);
                            Stores.ReplaceRange(resp);
                            //await page.Navigation.PushModalAsync(new EmptyList(mn));
                            //await page.Navigation.PopAsync();
                        }


                    }

                }
                catch (Exception ex)
                {
                    showAlert = true;

                }
                finally
                {
                    IsBusy = false;
                    GetJobListCommand.ChangeCanExecute();
                }

                if (showAlert)
                    await page.DisplayAlert("Oh Oooh :(", "Unable to gather rewards.", "OK");
            }
        }
        #endregion+

        #region GetOppotunities
        private Command oppotunitiesRefreshCommand;

        public Command OppotunitiesRefreshCommand
        {
            get
            {
                return oppotunitiesRefreshCommand ??
                    (oppotunitiesRefreshCommand = new Command(async () =>
                    {
                        ForceSync = true;
                        PageNumber += 1;
                        IsBusy = false;
                        await ExecuteGetOppotunitiesCommand();
                    }));
            }
        }

        private Command getOppotunitiesCommand;

        public Command GetOppotunitiesCommand
        {
            get
            {
                return getOppotunitiesCommand ??
                    (getOppotunitiesCommand = new Command(async () => await ExecuteGetOppotunitiesCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetOppotunitiesCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetJobListCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                // if (Stores != null)
                //    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "Loyalty";
                trn.Note = "Oppotunities";
                //trn.AgentCode = postSection.SupplierId;
                trn.Quantity = PageNumber;
                //trn.ServiceProvider = "Redeem Points";
                //trn.TransactionRef = "Mobile";
                //trn.ServiceId = postSection.ServiceId;
                // trn.Amount = 0;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }
                               
                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }
                                
                            }
                            else if (diff.TotalHours  >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) *(int) (diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else 
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes  < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }
                                
                            }
                            if (itm.SupplierId == "non")
                            {
                                itm.SupplierId = "Share Post";
                                itm.ThemeColor = "#79c606";
                                //itm.WebLink = "share.png";
                            }
                            else
                            {
                                itm.SupplierId = "Post Quote";
                                itm.ThemeColor = "#e2762b";
                                //itm.WebLink = "bid.png";
                            }
                        }
                        servics.OrderByDescending(u => u.date).ToList();
                        Stores.ReplaceRange(servics);
                    }
                    else
                    {
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "Be the first to post a request";
                        mn.Image = "http://192.168.100.150:5000/Content/Spani/Images/Oppotunity.jpg";
                        mn.HasProducts = false; // use it as show navigation
                        mn.IsEmptyList = true;
                        List<MenuItem> resp = new List<MenuItem>();
                        resp.Add(mn);
                        Stores.ReplaceRange(resp);
                        
                        //await page.Navigation.PushModalAsync(new EmptyList(mn));
                        //await page.Navigation.PopAsync();
                        //await page.Navigation.PopAsync();
                    }
                      
                }

            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetJobListCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to load oppotunities. Check your internet connectivity", "OK");

        }

        private async void RewardAlerts()
        {
            if (postSection.TransactionType > postSection.Count)
            {
                var answer = await page.DisplayAlert("Confirm", "Are you sure you want to award the " + postSection.Title + " Reward worth " + postSection.Count + " points?", "Yes", "No");
                if (answer == true)
                {
                    getOppotunitiesCommand.Execute(null);
                }
            }
            else
            {
                await page.DisplayAlert("Oh Oooh :(", "Your points are not enough for this this reward, you only have " + postSection.TransactionType + "points ", "OK");
            }
        }
        #endregion

        #region GetQuoted Jobs
        private Command quotedRefreshCommand;

        public Command QuotedRefreshCommand
        {
            get
            {
                return quotedRefreshCommand ??
                    (quotedRefreshCommand = new Command(async () =>
                    {
                        ForceSync = true;
                        MenuItem mm = new MenuItem();
                        mm.TransactionType = 0;
                        PageNumber += 1;
                        IsBusy = false;
                        await ExecuteGetQuotedCommand();
                    }));
            }
        }

        private Command getQuotedCommand;

        public Command GetQuotedCommand
        {
            get
            {
                return getQuotedCommand ??
                    (getQuotedCommand = new Command(async () => await ExecuteGetQuotedCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetQuotedCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetQuotedCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                // if (Stores != null)
                //    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "Loyalty";
                trn.Note = "MyQuotes";
                //trn.AgentCode = postSection.SupplierId;
                trn.Quantity = PageNumber;
                trn.ServiceProvider = "Redeem Points";
                trn.TransactionRef = "Mobile";
                //trn.ServiceId = postSection.ServiceId;
                trn.Amount = 0;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }

                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }

                            }
                            else if (diff.TotalHours >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) * (int)(diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }

                            }
                            itm.SupplierId = "View Quotes";
                        }
                            Stores.ReplaceRange(servics);
                    }
                    else
                    {
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "You have not posted a quote yet";
                        mn.Image = HostDomain + "/content/Spani/images/bid.jpg";
                        mn.HasProducts = false;
                        List<MenuItem> resp = new List<MenuItem>();
                        mn.IsEmptyList = true;
                        resp.Add(mn);
                        Stores.ReplaceRange(resp);
                        //await page.Navigation.PushModalAsync(new EmptyList(mn));
                       // await page.Navigation.PopAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                GetQuotedCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to post a quote. Check your internet connectivity", "OK");

        }

        #endregion

        #region GetQuotes
        public void GetBids(MenuItem mm)
        {
            myQuote = mm;
            OnPropertyChanged("myQuote");
            if (myQuote == null)
                return;

            if (ItemSelected == null)
            {
                GetBidCommand.Execute(null);
                myQuote = null;
            }
            else
            {
                ItemSelected.Invoke(myQuote);
            }
        }

        private Command getBidCommand;

        public Command GetBidCommand
        {
            get
            {
                return getBidCommand ??
                    (getBidCommand = new Command(async () => await ExecuteGetBidCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetBidCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetBidCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                // if (Stores != null)
                //    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "Work Space";
                trn.Note = "Bids";
                trn.Product = myQuote.Id;
                //trn.Quantity = postSection.Count;
                trn.ServiceProvider = "Request Quotes";
                trn.TransactionRef = "Mobile";
                //trn.ServiceId = postSection.ServiceId;
                trn.Amount = 0;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    servics.Where(u => u.SupplierId == uname).ToList();
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }

                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }

                            }
                            else if (diff.TotalHours >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) * (int)(diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }

                            }
                            itm.SupplierId = itm.SupplierId + "_" + itm.Title + "_" + itm.ServiceId + "_" + "Job" + "_" + itm.Id + "_" + trn.Product;
                        }
                        Stores.ReplaceRange(servics);
                    }
                    else
                    {
                        
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "You have no quoted requests";
                        mn.Image = "http://192.168.100.150:5000/content/Spani/images/bid.jpg";
                        mn.HasProducts = false;
                        mn.Title = "Quotes";
                        mn.IsEmptyList = true;
                        List<MenuItem> resp = new List<MenuItem>();
                        resp.Add(mn);
                        Stores.ReplaceRange(resp);
                        //await page.Navigation.PopModalAsync();
                        //await page.Navigation.PushModalAsync(new EmptyList(mn));
                        
                    }
                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                GetBidCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather quotes. Check your internet connectivity", "OK");

        }

        #endregion

        #region GetRequested Bids

        private Command getRequestBidCommand;

        public Command GetRequestBidCommand
        {
            get
            {
                return getRequestBidCommand ??
                    (getRequestBidCommand = new Command(async () => await ExecuteRequestBidCommand(myRequestQuotes), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteRequestBidCommand(MenuItem selecteditm)
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetRequestBidCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                // if (Stores != null)
                //    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "Loyalty";
                trn.Note = "Bids";
                trn.Product = selecteditm.Id;
                //trn.Quantity = postSection.Count;
                trn.ServiceProvider = "Redeem Points";
                trn.TransactionRef = "Mobile";
                // trn.ServiceId = postSection.ServiceId;
                trn.Amount = 0;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }

                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }

                            }
                            else if (diff.TotalHours >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) * (int)(diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }

                            }
                            itm.SupplierId = "View Quote";
                        }
                            Stores.ReplaceRange(servics);
                        
                    }
                    else
                    {
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "You have no request quotations";
                        mn.Image = "http://192.168.100.150:5000/content/Spani/images/bid.jpg";
                        mn.HasProducts = false;
                        mn.Title = "Quotations";
                        //List<MenuItem> resp = new List<MenuItem>();
                        //resp.Add(mn);
                        //Stores.ReplaceRange(resp);
                        await page.Navigation.PushModalAsync(new EmptyList(mn));
                        await page.Navigation.PopAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                GetRequestBidCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to Collect bids for this Job. Check your internet connectivity", "OK");

        }

        public async Task ExecuteRequestBidAsync(MenuItem selecteditm)
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetRequestBidCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                // if (Stores != null)
                //    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = "Loyalty";
                trn.Note = "Bids";
                trn.Product = selecteditm.Id;
                //trn.Quantity = postSection.Count;
                trn.ServiceProvider = "Redeem Points";
                trn.TransactionRef = "Mobile";
                // trn.ServiceId = postSection.ServiceId;
                trn.Amount = 0;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    if (servics.Count > 0)
                    {
                        foreach (var itm in servics)
                        {
                            var diff = DateTime.Now.Subtract(itm.date);
                            if (diff.TotalDays > 30)
                            {
                                var days = 0;
                                if (diff.Days < 0)
                                { days = (-1) * (diff.Days); }
                                else { days = diff.Days; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = itm.date.ToString(" dd MMM yyyy");
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + itm.date.ToString(" dd MMM yyyy");
                                }

                            }
                            else if (diff.TotalDays >= 1)
                            {
                                var days = 0;
                                if (diff.TotalDays < 0)
                                { days = (-1) * (int)(diff.TotalDays); }
                                else { days = (int)diff.TotalDays; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = days + " days ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + days + " days ago";
                                }

                            }
                            else if (diff.TotalHours >= 1)
                            {
                                var Hours = 0;
                                if (diff.TotalHours < 0)
                                { Hours = (-1) * (int)(diff.TotalHours); }
                                else { Hours = (int)diff.TotalHours; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Hours + " hrs ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Hours + " hrs ago";
                                }
                            }
                            else
                            {
                                var Minutes = 0;
                                if (diff.TotalMinutes < 0)
                                { Minutes = (-1) * (int)(diff.TotalMinutes); }
                                else { Minutes = (int)diff.TotalMinutes; }
                                if (string.IsNullOrEmpty(itm.Note))
                                {
                                    itm.Note = Minutes + " mins ago";
                                }
                                else
                                {
                                    itm.Note = itm.Note.Trim() + " " + Minutes + " mins ago";
                                }

                            }
                            itm.SupplierId = "View Quote";

                        }
                        Stores.ReplaceRange(servics);

                    }
                    else
                    {
                        MenuItem mn = new YomoneyApp.MenuItem();
                        mn.Description = "You have no request quotations";
                        mn.Image = "http://192.168.100.150:5000/content/Spani/images/bid.jpg";
                        mn.HasProducts = false;
                        mn.Title = "Request Quotes";
                        List<MenuItem> resp = new List<MenuItem>();
                        resp.Add(mn);
                        Stores.ReplaceRange(resp);
                        await page.Navigation.PopAsync();
                        await page.Navigation.PushModalAsync(new EmptyList(mn));
                        
                    }
                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                GetRequestBidCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to Collect bids for this Job. Check your internet connectivity", "OK");

        }

        #endregion

        #region save Request
        Command saveRequestCommand;
        public Command SaveRequestCommand
        {
            get
            {
                return saveRequestCommand ??
                    (saveRequestCommand = new Command(async () => await ExecuteSaveRequestCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSaveRequestCommand()
        {
            if (IsBusy)
                return;
           /* if (string.IsNullOrWhiteSpace(Ptitle))
            {
                await page.DisplayAlert("Enter Title", "Please enter a the title of your request .", "OK");
                return;
            }*/
            if (string.IsNullOrWhiteSpace(Description))
            {
                await page.DisplayAlert("Enter Description", "Please enter a breif description of the service you need.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Enter Budget", "Please enter a budget for the job post.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();

                #region JobPost
                JobPost jp = new YomoneyApp.JobPost();
                jp.CustomerName = Ptitle;
                jp.Category  = Subcategory;
                jp.Description = Description;
                jp.Title = Category;
                jp.Budget  = decimal.Parse(Budget);
                
                jp.ClosingDate = date.ToUniversalTime();
                //jp.Title = 
                #endregion

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "CreateJobRequest";
                trn.Currency = Currency;
                //trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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
                Body += "&Currency=" + trn.Currency;

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000" || response.ResponseCode == "Success")
                    {
                        //await page.Navigation.PopModalAsync();
                        await App.Current.MainPage.Navigation.PopModalAsync();
                        await page.Navigation.PushAsync(new SpaniWorkSpace());
                    }
                    else
                    {
                        await page.DisplayAlert("Job Request", "Unable to save Job Request, please try again.", "OK");
                    }
                   // await page.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error :(", "Unable to save Job Request, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                saveProfileCommand?.ChangeCanExecute();
            }

           

        }
        #endregion

        #region  Profile
        JobProfile createdProfile;
        public JobProfile CreatedProfile
        {
            get { return createdProfile; }
            set
            {
                createdProfile = value;
                OnPropertyChanged("CreatedProfile");
                if (postSection == null)
                    return;

                if (ItemSelected == null)
                {
                    RewardAlerts();
                    CreatedProfile = null;  
                }
                else
                {
                    ItemSelected.Invoke(postSection);
                }
            }
        }

        Command saveProfileCommand;

        public Command SaveProfileCommand
        {
            get
            {
                return saveProfileCommand ??
                    (saveProfileCommand = new Command(async () => await ExecuteSaveProfileCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSaveProfileCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Ptitle))
            {
                await page.DisplayAlert("Enter Company", "Please enter a trade name or your name is you are not a company.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                await page.DisplayAlert("Enter Description", "Please enter a breif description of what you do.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Address))
            {
                await page.DisplayAlert("Enter Address", "Please enter an opperational address.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();

                #region JobPost
                JobProfile jp = new YomoneyApp.JobProfile();
                jp.CustomerName = Ptitle;
                jp.JobCategory = Category;
                jp.Description = Description;
                jp.SubCategory = Subcategory;
                jp.Address = Address;
                //jp.Title = 
                #endregion

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "CreateServiceProfile";
                //trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if(response.ResponseCode == "00000" || response.ResponseCode == "Success")
                    {
                        //await page.Navigation.PopModalAsync();
                        await App.Current.MainPage.Navigation.PopModalAsync();
                        await page.Navigation.PushAsync(new SpaniWorkSpace());
                    }
                    else
                    {
                        await page.DisplayAlert("Job Profile", "Unable to save Job Profile, please try again.", "OK");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Job Profile", "Unable to save Job Profile, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                saveProfileCommand?.ChangeCanExecute();
            }
        }

        Command getProfileCommand;

        public Command GetProfileCommand
        {
            get
            {
                return getProfileCommand ??
                    (getProfileCommand = new Command(async () => await ExecuteGetProfileCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteGetProfileCommand()
        {
            if (IsBusy)
                return;
            

            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();
                
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
               // trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "GetServiceProfile";
                //trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        //await page.Navigation.PopModalAsync();
                        await App.Current.MainPage.Navigation.PopModalAsync();
                        await page.Navigation.PushAsync(new SpaniWorkSpace());
                    }
                    else
                    {
                        await page.DisplayAlert("Job Profile", "Unable to view Job Profile, please try again.", "OK");
                    }

                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Job Profile", "Unable to view Job Profile, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                getProfileCommand?.ChangeCanExecute();
            }

            //  await page.Navigation.PopAsync();

        }

        #endregion

        #region Post Bid

        Command postBidCommand;
        public Command PostBidCommand
        {
            get
            {
                return postBidCommand ??
                    (postBidCommand = new Command(async () => await ExecutePostBidCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecutePostBidCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Enter Budget", "Please enter a budget for the job post.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                await page.DisplayAlert("Enter Description", "Please enter a breif description of what you do.", "OK");
                return;
            }
           

            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();

                #region JobBid
                JobBid  jp = new YomoneyApp.JobBid();
                
                jp.Amount = decimal.Parse(Budget);
                jp.Description = Description;
                jp.JobPostId = JobPostId;

                //jp.Title = 
                #endregion

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "CreateBid";
                trn.Currency = Currency;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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
                Body += "&Currency=" + trn.Currency;

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                   // var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Message = "Job Bid Posted successfully";
                    await page.DisplayAlert("Success", "Job Bid Posted successfully", "OK");
                    //Stores.ReplaceRange(servics);

                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", "Unable to save Quote, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                PostBidCommand?.ChangeCanExecute();
            }
            await App.Current.MainPage.Navigation.PopModalAsync();
            //await page.Navigation.PopModalAsync();

        }
        #endregion

        #region AwardBid
        Command awardBidCommand;
        public Command AwardBidCommand
        {
            get
            {
                return awardBidCommand ??
                    (awardBidCommand = new Command(async () => await ExecuteAwardBidCommand(), () => { return !IsBusy; }));
            }
        }
        async Task ExecuteAwardBidCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Enter Budget", "Please enter a budget for the job post.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                await page.DisplayAlert("Enter Description", "Please enter a breif description of what you do.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();

                #region JobBid
                JobBid jp = new YomoneyApp.JobBid();
                
                jp.Amount = decimal.Parse(Budget);
                jp.Description = Description;
                jp.JobPostId = JobPostId;
                jp.Currency = Currency;
                //jp.Title = 
                #endregion

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = ProcessingCode;
                trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "AwardBid";
                //trn.AgentCode = selectedOption.SupplierId;
                // trn.Quantity = selectedOption.Count;
                // trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                     var servics = JsonConvert.DeserializeObject<MenuItem>(response.Narrative);
                    if (response.ResponseCode == "00000")
                    {
                        Message = "Job Awarded Successfully";
                        await page.DisplayAlert("Success :)", "You Have successfully awarded this job to " + servics.Title, "OK");
                    }
                    else if(response.ResponseCode == "11102")
                    {
                        await page.DisplayAlert("Error :)", response.Description, "OK");
                    }
                    else
                    {
                        await page.DisplayAlert("Error :)", "Job Could not be awarded please try again later " , "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Oh Oooh :(", "Unable to save feedback, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                awardBidCommand?.ChangeCanExecute();
            }

            

        }

        public void GetAwardBid(INavigation navigation, MenuItem mm)
        {
          //  MenuItem mm = JsonConvert.DeserializeObject<MenuItem>(selecteditem);
            myQuote = mm;
            currentPage = navigation;
            OnPropertyChanged("myQuote");
            OnPropertyChanged("currentPage");
            if (myQuote == null)
                return;

            if (ItemSelected == null)
            {
                //string myinput = await InputBox(this.Navigation, "Payment", result.Text);
                ConfirmAwardBidCommand.Execute(null);
               // myQuote = null;
            }
            else
            {
                ItemSelected.Invoke(myQuote);
            }
        }

        Command confirmAwardBidCommand;
        public Command ConfirmAwardBidCommand
        {
            get
            {
                return confirmAwardBidCommand ??
                    (confirmAwardBidCommand = new Command(async () => await ExecuteConfirmAwardBidCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteConfirmAwardBidCommand()
        {
            if (IsBusy)
                return;
           
            IsBusy = true;

            try
            {
                TransactionRequest trn = new RetailKing.Models.TransactionRequest();
                string myinput = await InputBox(currentPage, "Award Bid", myQuote.Description, myQuote.Title );
                if (myinput == "Yes")
                {
                    MenuItem mn = new YomoneyApp.MenuItem();
                    char[] delimiter = myQuote.Currency.ToCharArray();
                    var amt = myQuote.Amount.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    mn.Amount = String.Format("{0:n}", (decimal.Parse(amt[1]) * decimal.Parse("0.05")).ToString());
                    mn.Title = "Pay 5% of job to Award";
                    myQuote.Amount = amt[1];
                   await page.Navigation.PushModalAsync(new PaymentPage(mn));
                  
                    IsBusy = false;
                    MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (sender, arg) =>
                     {
                      if (arg == "Payment Success")
                       {
                            #region JobBid
                            JobBid jp = new YomoneyApp.JobBid();
                            char[] delimit = new char[] { '_' }; ;
                            var jb = myQuote.SupplierId.Split(delimit, StringSplitOptions.RemoveEmptyEntries);

                            jp.Amount = decimal.Parse(myQuote.Amount);
                            jp.Description = myQuote.Description;
                            jp.JobPostId = jb[5];// JobPostId;
                            jp.RequesterId = myQuote.SupplierId;

                            //jp.Title = 
                            #endregion

                            AccessSettings acnt = new Services.AccessSettings();
                            string pass = acnt.Password;
                            string uname = acnt.UserName;
                            trn.CustomerAccount = uname + ":" + pass;
                            trn.MTI = "0200";
                            trn.ProcessingCode = ProcessingCode;
                            trn.Narrative = JsonConvert.SerializeObject(jp);
                            trn.Note = "AwardBid";
                            trn.Amount = jp.Amount;
                            //trn.AgentCode = selectedOption.SupplierId;
                            // trn.Quantity = selectedOption.Count;
                            // trn.Product = selectedOption.Description;
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

                            var client = new HttpClient();
                            var myContent = Body;
                            string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                            string result = await client.GetStringAsync(paramlocal);
                            if (result != "System.IO.MemoryStream")
                            {
                                var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                                //var servics = JsonConvert.DeserializeObject<MenuItem>(response.Narrative);
                                if (response.ResponseCode == "00000")
                                {
                                    Message = "Job Awarded Successfully";
                                    await page.DisplayAlert("Success", "You Have successfully awarded this job to " + myQuote.Title, "OK");
                                }
                                else if (response.ResponseCode == "11102")
                                {
                                    await page.DisplayAlert("Job Award Error", response.Description, "OK");
                                }
                                else
                                {
                                    await page.DisplayAlert("Job Award Error ", "Job Could not be awarded please try again later ", "OK");
                                }
                                try
                                {
                                     await App.Current.MainPage.Navigation.PopModalAsync();
                                     //page.Navigation.PopModalAsync();
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            await page.DisplayAlert("Payment Failed", "Unable to Award Job, the 5% commitment fee was not paid.", "OK");
                        }
                    });
                 }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", "Unable to Award Job, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                awardBidCommand?.ChangeCanExecute();
            }

        }

        public static Task<string> InputBox(INavigation navigation, string Title, string message, string client)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();

            var lblTitle = new Label { Text = Title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold,FontSize = 40, Margin=10 };
            var lblMessage = new Label { Text = message, HorizontalOptions = LayoutOptions.CenterAndExpand};
            var lblClient = new Label { Text =client, TextColor = Color.FromHex("#e2762b") , HorizontalOptions = LayoutOptions.CenterAndExpand };
           // var txtInput = new Entry { Text = "" };

            var btnOk = new Button
            {
                Text = "Ok",
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                BackgroundColor = Color.Green,
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await navigation.PopModalAsync();
                // pass result
                tcs.SetResult("Yes");
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 100,
                BackgroundColor = Color.FromHex("#e2762b")
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                //await navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblMessage,  slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            //txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }
        #endregion

        #region Awarded Bid
        Command getAwardedCommand;
        public Command GetAwardedCommand
        {
            get
            {
                return getAwardedCommand ??
                    (getAwardedCommand = new Command(async () => await ExecuteAwardBidCommand(), () => { return !IsBusy; }));
            }
        }
        async Task ExecuteGetAwardedCommand()
        {
            if (IsBusy)
                return;

            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = ProcessingCode;
                //trn.Narrative = JsonConvert.SerializeObject(jp);
                trn.Note = "AwardBid";
                //trn.AgentCode = selectedOption.SupplierId;
                //trn.Quantity = selectedOption.Count;
                //trn.Product = selectedOption.Description;
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

                var client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<MenuItem>(response.Narrative);
                    Message = "Job Awarded Successfully";
                    await page.DisplayAlert("Success :)", "You Have successfully awarded this job to " + servics.Title, "OK");
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Oh Oooh :(", "Unable to save feedback, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                awardBidCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region Object 

        double longitude ;
        public double Longitude
        {
            get { return longitude; }
            set { SetProperty(ref longitude, value); }
        }

        double latitude;
        public double Latitude
        {
            get { return latitude; }
            set { SetProperty(ref latitude, value); }
        }

        string sessionId = "";
        public string SessionId
        {
            get { return sessionId; }
            set { SetProperty(ref sessionId, value); }
        }

        int pageNumber = 1;
        public int PageNumber
        {
            get { return pageNumber; }
            set { SetProperty(ref pageNumber, value); }
        }

        string currency = string.Empty;
        public string Currency
        {
            get { return currency; }
            set { SetProperty(ref currency, value); }
        }

        string budget = string.Empty;
        public string Budget
        {
            get { return budget; }
            set { SetProperty(ref budget, value); }
        }

        bool showNavigation = false;
        public bool ShowNavigation
        {
            get { return showNavigation; }
            set { SetProperty(ref showNavigation, value); }
        }
        
        string phone = string.Empty;
        public string PhoneNumber
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        string action = string.Empty;
        public string Action
        {
            get { return action; }
            set { SetProperty(ref action, value); }
        }

        string category = string.Empty;
        public string Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }

        string ptitle = string.Empty;
        public string Ptitle
        {
            get { return ptitle; }
            set { SetProperty(ref ptitle, value); }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        string subcategory = string.Empty;
        public string Subcategory
        {
            get { return subcategory; }
            set { SetProperty(ref subcategory, value); }
        }

        string label1 = string.Empty;
        public string Label1
        {
            get { return label1; }
            set { SetProperty(ref label1, value); }
        }
        string label2 = string.Empty;
        public string Label2
        {
            get { return label2; }
            set { SetProperty(ref label2, value); }
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

        string jobPostId = string.Empty;
        public string JobPostId
        {
            get { return jobPostId; }
            set { SetProperty(ref jobPostId, value); }
        }

        int serviceType = 4;
        public int ServiceType
        {
            get { return serviceType; }
            set
            {
                SetProperty(ref serviceType, value);
            }
        }

        int rating = 10;
        public int Rating
        {
            get { return rating; }
            set
            {
                SetProperty(ref rating, value);
            }
        }

        DateTime date = Today;
        public DateTime Date
        {
            get { return date; }
            set
            {
                SetProperty(ref date, value);
            }
        }

       
        #endregion

    }

}

