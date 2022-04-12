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


namespace YomoneyApp.ViewModels.Geo
{
    [System.ComponentModel.DesignTimeVisible(false)]
    
    public class GeoLocationViewModel : ViewModelBase
    {
        public SQLiteConnection conn;
        public DBStorageModel storageModel;
        readonly string HostDomain = "http://192.168.100.150:5000";
        bool showAlert = false;

        public ObservableRangeCollection<MenuItem> OrderedItems { get; set; }
        public ObservableRangeCollection<MenuItem> ItemsList { get; set; }
        public Action<MenuItem> PackageSelected { get; set; }
        //Directions directions;
        public GeoLocationViewModel(Page page) : base(page)
        {
            OrderedItems = new ObservableRangeCollection<MenuItem>();
            ItemsList = new ObservableRangeCollection<MenuItem>();
            conn = DependencyService.Get<Isqlite>().GetConnection();
            conn.CreateTable<DBStorageModel>();
            
            //directions = new Directions(RouteId,RouteName,Role, RouteRate, RouteCost, RouteDuration, 
            //    RouteDistance, RouteRealTimeDistance, RouteRealTimeInstructions);
        }

        #region Using Shared Preferences
        public void AddValue(string key, string value)
        {
            Preferences.Set(key, value);
        }
        #endregion

        public List<MenuItem> packageDetails = new List<MenuItem>()
            {
                new MenuItem{ Title = "Item1", Description = "Item1 Description", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", IsDelivered= false},
                new MenuItem{ Title = "Item2", Description = "Item2 Description", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", IsDelivered= false},
                new MenuItem{ Title = "Item3", Description = "Item3 Description", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", IsDelivered= false},
                new MenuItem{ Title = "Item4", Description = "Item4 Description", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", IsDelivered= false},
                new MenuItem{ Title = "Item5", Description = "Item5 Description", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", IsDelivered= false}
            };

        Command getDemoOrderDetailsCommand;
        public Command GetDemoOrderDetailsCommand
        {
            get
            {
                return getDemoOrderDetailsCommand ??
                    (getDemoOrderDetailsCommand = new Command(async () => await ExecuteGetDemoOrderDetailsCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetDemoOrderDetailsCommand()
        {
            ItemsList.ReplaceRange(packageDetails);
        }        

        #region Get Packages from the Server

        Command getPackageDetails;
        public Command getPackageDetailsCommand
        {
            get
            {
                return getPackageDetails ??
                    (getPackageDetails = new Command(async () => await ExecuteGetPackageDetailsCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetPackageDetailsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = false;

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "Product";
                trn.TransactionType = 42;
                trn.Note = "";
                trn.Product = RouteId;
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "";
                trn.TransactionRef = "";
                trn.ServiceId = 0;// redeemSection.ServiceId;
                trn.Amount = 0;
                string Body = "";
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        //AddValue("StoredProducts", response.Narrative);

                        DBStorageModel dBStorage = new DBStorageModel();
                        dBStorage.ProductLines = response.Narrative;

                        int x = 0;

                        try
                        {
                            x = conn.Insert(dBStorage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                        }

                        if (x == 1)
                        {
                            await page.DisplayAlert("Success!", "Packages saved successfully!", "Ok");

                            //directions.DisplayRoutes();
                        }
                        else
                        {
                            await page.DisplayAlert("Error!", "There are no delivery packages on this route.", "Ok");

                           // directions.DisplayRoutes();
                        }

                        //var menuItems = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        //menuItems.ToArray();

                        //OrderedItems.ReplaceRange(menuItems);
                    }
                    else
                    {
                        await page.DisplayAlert("Error!", "Failed to receive Package Details from the server! Cancel the trip and open your Tasks Tab again", "Ok");
                    }
                }
                else
                {
                    await page.DisplayAlert("Error!", "Failed to receive Package Details from the server! Cancel the trip and open your Tasks Tab again", "Ok");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                await page.DisplayAlert("Error!", "Failed to receive Package Details from the server! Cancel the trip and open your Tasks Tab again", "Ok");
            }
        }

        #endregion

        #region Retrive Packages Saved on phone

        Command getPackagesSavedCommand;
        public Command GetPackagesSavedCommand
        {
            get
            {
                return getPackagesSavedCommand ??
                    (getPackagesSavedCommand = new Command(async () => await ExecuteGetPackagesSavedCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetPackagesSavedCommand()
        {
            if (IsBusy)
                return;

            IsBusy = false;

            var packageDetails = (from x in conn.Table<DBStorageModel>() select x).FirstOrDefault();

            var menuItems = JsonConvert.DeserializeObject<List<MenuItem>>(packageDetails.ToString());
            // menuItems.ToArray();

            //ItemsList.Add(menuItems);
            OrderedItems.ReplaceRange(menuItems);
        }

        #endregion

        #region Get PackageLines from PackageSelected

        MenuItem selectedPackage;

        public MenuItem SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                selectedPackage = value;
                OnPropertyChanged("SelectedPackage");

                if (selectedPackage == null)
                    return;

                if (SelectedPackage == null)
                {
                    //selected chat bid
                    //var Id = SelectedChat.ReceiverId + "_" + SelectedChat.ReceiverName + "_" + SelectedChat.AgentId + "_" + "Job" + "_" + SelectedChat.BidId + "_" + SelectedChat.JobId;

                    SelectedPackage = null;
                    SelectedPackage = null;
                }
                else
                {
                    PackageSelected.Invoke(selectedPackage);
                }
            }
        }

        public void GetPackageLinesSelected(MenuItem menuItem)
        {
            selectedPackage = menuItem;
            OnPropertyChanged("SelectedPackage");
            if (selectedPackage == null)
                return;

            if (selectedPackage == null)
            {
                selectedPackage = null;
                selectedPackage = null;
            }
            else
            {
                PackageSelected.Invoke(selectedPackage);
            }
        }

        Command getPackageLinesSelectedCommand;
        public Command GetPackageLinesSelectedCommand
        {
            get
            {
                return getPackageLinesSelectedCommand ??
                    (getPackageLinesSelectedCommand = new Command(async () => await ExecuteGetPackageLinesSelectedCommand(selectedPackage), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetPackageLinesSelectedCommand(MenuItem menuItem)
        {
            if (IsBusy)
                return;

            IsBusy = false;

            var packageLines = JsonConvert.DeserializeObject<List<MenuItem>>(menuItem.Note);
            packageLines.ToArray();

            ItemsList.ReplaceRange(packageLines);
        }

        #endregion

        #region Save Packages Choices 

        Command saveChoicesCommand;
        public Command SaveChoicesCommand
        {
            get
            {
                return saveChoicesCommand ??
                    (saveChoicesCommand = new Command(async () => await ExecuteSaveChoicesCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSaveChoicesCommand()
        {
            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                //trn.Narrative = "Product";
                trn.TransactionType = 42;
                trn.Note = "";
                //trn.Product = RouteId;
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "";
                trn.TransactionRef = "";
                trn.ServiceId = 0;// redeemSection.ServiceId;
                trn.Amount = 0;
                string Body = "";
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        //AddValue("StoredProducts", response.Narrative);

                        DBStorageModel dBStorage = new DBStorageModel();
                        dBStorage.ProductLines = response.Narrative;

                        int x = 0;

                        try
                        {
                            x = conn.Insert(dBStorage);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);
                        }

                        if (x == 1)
                        {
                            await page.DisplayAlert("Success!", "Packages saved successfully!", "Ok");
                        
                        }
                        else
                        {
                            await page.DisplayAlert("Error!", "Failed to save packages! Open Tasks and repeat the process, something could have gone wrong.", "Ok");
                        }

                        //var menuItems = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        //menuItems.ToArray();

                        //OrderedItems.ReplaceRange(menuItems);
                    }
                    else
                    {
                        await page.DisplayAlert("Error!", "Failed to receive Package Details from the server! Cancel the trip and open your Tasks Tab again", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

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

        string routeId = string.Empty;
        public string RouteId
        {
            get { return routeId; }
            set { SetProperty(ref routeId, value); }
        }

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

        //List<MenuItem> itemsList;
        //public List<MenuItem> ItemsList
        //{
        //    get { return itemsList; }
        //    set { SetProperty(ref itemsList, value); }
        //}
    }
}
