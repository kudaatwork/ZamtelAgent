using MvvmHelpers;
using YomoneyApp.Services;
using YomoneyApp.Views;
using YomoneyApp.Views.Login;
using YomoneyApp.Views.Spani;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.Toasts;
using YomoneyApp.Models;
using System.Linq;
using YomoneyApp.Models.Image;
using System.Web;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Services;
using Xamarin.Forms.GoogleMaps;

namespace YomoneyApp
{
    public class HomeViewModel : ViewModelBase
    {
        public static FileUpload fileUpload = new FileUpload();

        readonly string HostDomain = "https://www.yomoneyservice.com";
        bool showAlert = false;
        string Latitude = "";
        string Longitude = "";
        public ObservableRangeCollection<MenuItem> myItemsSource { get; set; }
        public ObservableRangeCollection<MenuItem> myButtonSource { get; set; }
        public MenuItem accData { get; set; }

        public HomeViewModel(Page page) : base(page)
        {
            Title = "YoApp";
            Position = 0;
            myItemsSource = new ObservableRangeCollection<MenuItem>();
            myButtonSource = new ObservableRangeCollection<MenuItem>();

            accData = new MenuItem();
            //TemplateSelector = new MyTemplateSelector(); //new DataTemplate (typeof(MyView));            
        }
        public int Position { get; set; }
     
        private Command getStoresCommand;

        public Command GetStoresCommand
        {
            get
            {
                return getStoresCommand ??
                    (getStoresCommand = new Command(async () => await ExecuteGetStoresCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetStoresCommand()
        {
            // advertising request 
            if (IsBusy)
                return;

            IsBusy = false;
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
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "460000";
                trn.Narrative = "Home";
                trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "HOME";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = 0;// redeemSection.ServiceId;
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
                Body += "&Mpin=" + trn.Mpin;
                Body += "&Note=" + trn.Note;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        servics.ToArray();
                        myItemsSource.ReplaceRange(servics);
                    }
                    else if (response.ResponseCode == "12")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        ac.DeleteCredentials();
                        await page.Navigation.PushAsync(new AccountMain());

                    }
                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
        }

        #region menus
        private Command getMenusCommand;

        public Command GetMenusCommand
        {
            get
            {
                return getMenusCommand ??
                    (getMenusCommand = new Command(async () => await ExecuteGetMenusCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetMenusCommand()
        {
            if (IsBusy)
                return;

            IsBusy = false;

            try
            {
                myButtonSource.Clear();
                MenuItem mnu = new MenuItem();
                mnu.Title = "Bill Payments";
                mnu.Image = "payments_icon.png";
                mnu.ServiceId = 11;
                mnu.SupplierId = "All";
                mnu.Section = "Yomoney";
                mnu.TransactionType = 1;

                myButtonSource.Add(mnu);
                mnu = new MenuItem();
                mnu.Title = "Airtime";
                mnu.Image = "airtime_icon.png";
                mnu.ServiceId = 7;
                mnu.SupplierId = "All";
                mnu.Section = "Yomoney";
                mnu.TransactionType = 2;

                myButtonSource.Add(mnu);
                mnu = new MenuItem();
                mnu.Title = "Jobs";
                mnu.Image = "jobs_icon.png";
                mnu.Section = "Yomoney";
                mnu.TransactionType = 2;

                myButtonSource.Add(mnu);
                mnu = new MenuItem();
                mnu.Title = "Promotions";
                mnu.Image = "promotions_icon.png";
                mnu.TransactionType = 2;
                mnu.Section = "Yomoney";
                myButtonSource.Add(mnu);

                mnu = new MenuItem();
                mnu.Title = "Tasks";
                mnu.Image = "home_tasks.png";
                mnu.ServiceId = 1;
                mnu.SupplierId = "All";
                mnu.Section = "5-0001-0000000";
                mnu.TransactionType = 6;
                mnu.Description = "YoLifestyle";
                myButtonSource.Add(mnu);

                mnu = new MenuItem();
                mnu.Title = "Services";
                mnu.Image = "services_icon.png";
                mnu.TransactionType = 12;
                mnu.Section = "Supplier Services";
                mnu.ServiceId = 12;
                mnu.SupplierId = "All";
                mnu.Section = "Yomoney";
                myButtonSource.Add(mnu);

                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "490000";
                trn.Narrative = "Home";
                trn.Note = Longitude + "," + Latitude;//"-122.084,37.4219983333333"; //
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "HOME";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = 0;// redeemSection.ServiceId;
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
                Body += "&Mpin=" + trn.Mpin;
                Body += "&Note=" + trn.Note;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        servics.ToArray();
                        myButtonSource.AddRange(servics);
                    }
                }
            }
            catch (Exception ex)
            {
                showAlert = true;
            }
        }
        #endregion

        #region Webview Stuff

        public static GoogleDirection googleDirectionGlobal;

        public static List<RoutesInfo> routes = new List<RoutesInfo>();

        static string originAddress = string.Empty;
        static string destinationAddress = string.Empty;

        public static List<Position> polygonLocationPoints = new List<Position>
        {
            new Position(-17.822804831359214,31.044133453637883),
            new Position(-17.82215113840043,31.04730918913408),
            new Position(-17.824275631749476,31.048939972226727),
            new Position(-17.825828130102487,31.047738342579514),
            new Position(-17.825828130102487,31.044991760528745)
        };

        #region Display Map with points from Server
        public async void DisplayMap(string serverData)
        {
            try
            {
                string dencodedServerData = HttpUtility.HtmlDecode(serverData);

                char[] delimite = new char[] { '_' };

                string[] parts = dencodedServerData.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 5 || parts.Length <= 1)
                {
                    CheckData(serverData);
                }
                else
                {
                    var routeId = parts[0];
                    RouteId = parts[0];
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

                    if (rate >= 0) // Only if Rate is Greater than 0
                    {
                  //      if (role == "driver" || role == "passenger") // Drivers and Passengers
                  //      {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.Navigation.PushAsync(new Directions(RouteId, RouteName, Role, RouteRate, RouteCost, RouteDuration,
                                   RouteDistance, RouteRealTimeDistance, RouteRealTimeInstructions));

                                // await App.Current.MainPage.Navigation.PushAsync(new PolylognView());
                            });
            //            }
            ///           else
            //            {
            //                Device.BeginInvokeOnMainThread(async () =>
            //                {
            //                    await App.Current.MainPage.Navigation.PushAsync(new Viewers(RouteName, Role, RouteRate, RouteCost, RouteDuration,
            //                        RouteDistance, RouteRealTimeDistance, RouteRealTimeInstructions));
            //                });
            //            }
                    }
                    else
                    {
                        await page.DisplayAlert("Error", "There isn't any route rate alloted to this trip. Please contact your service provider for more information", "Ok");

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new HomePage());
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await page.DisplayAlert("Error", "There has been an error in loading your routes from the server. " +
                    "Contact customer support from more information", "Ok");

                await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
            }
        }

        public async void CheckData(string serverData)
        {
            var dencodedServerData = string.Empty;

            if (!String.IsNullOrEmpty(serverData))
            {
                dencodedServerData = HttpUtility.HtmlDecode(serverData);
            }

            char[] delimite = new char[] { '_' };

            string[] parts = dencodedServerData.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 5)
            {
                var purpose = parts[0].ToUpper().Trim();
                var supplier = parts[1];
                var serviceId = parts[2];
                var actionId = parts[3];
                var formId = parts[4];
                var fieldId = parts[5];
                var phoneNumber = parts[6];

                MenuItem menuItem = new MenuItem();

                switch (purpose)
                {
                    case "SIGNATURE":

                        fileUpload.Purpose = purpose;
                        fileUpload.SupplierId = supplier;
                        fileUpload.ServiceId = Convert.ToInt64(serviceId);
                        fileUpload.ActionId = Convert.ToInt64(actionId);
                        fileUpload.FormId = formId;
                        fileUpload.FieldId = fieldId;
                        fileUpload.PhoneNumber = phoneNumber;

                        //await serviceViewModel.ExecuteRenderActionCommand(null);

                        menuItem.ActionId = fileUpload.ActionId;
                        menuItem.ServiceId = fileUpload.ServiceId;
                        menuItem.SupplierId = fileUpload.SupplierId;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new SignaturePage(menuItem));
                        });

                        //await page.Navigation.PushAsync(new SignaturePage(null));

                        break;

                    case "UPLOAD":
                        break;

                    case "ROUTE":
                        DisplayMap(serverData);
                        break;

                    case "PAYMENT":
                        break;
                    default:
                        break;
                }
            }
            else if (parts.Length == 1)
            {
                var formPurpose = parts[0].ToUpper().Trim();

                if (formPurpose == "BACK")
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.Navigation.PopAsync();
                    });
                }
            }
            else
            {

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
        #endregion

        #region get location
        private Command getGeolocationCommand;

        public Command GetGeolocationCommand
        {
            get
            {
                return getGeolocationCommand ??
                    (getGeolocationCommand = new Command(async () => await ExecuteGetGeolocationCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetGeolocationCommand()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Latitude = "Latitude: " + location.Latitude.ToString();
                    Longitude = "Longitude:" + location.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
            }
        }
        #endregion

        #region Currencies

        private Command getCurrencyCommand;

        public Command GetCurrencyCommand
        {
            get
            {
                return getCurrencyCommand ??
                    (getCurrencyCommand = new Command(async () => await ExecuteGetCurrencyCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetCurrencyCommand()
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
                trn.ProcessingCode = "470000";
                trn.Narrative = "Currency";
                trn.Note = "";
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "Currency";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = 0;// redeemSection.ServiceId;
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
                Body += "&Mpin=" + trn.Mpin;
                Body += "&Note=" + trn.Note;


                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        servics.ToArray();
                        myItemsSource.ReplaceRange(servics);
                    }
                    else if (response.ResponseCode == "12")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        ac.DeleteCredentials();
                        await page.Navigation.PushAsync(new AccountMain());

                    }

                }

            }
            catch (Exception ex)
            {
                showAlert = true;
            }
        }
        #endregion

        #region Wallet 
        private Command getWalletCommand;

        public Command GetWalletCommand
        {
            get
            {
                return getWalletCommand ??
                    (getWalletCommand = new Command(async () => await ExecuteGetWalletCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetWalletCommand()
        {
            if (page.Navigation.NavigationStack.Count == 0 ||
                    page.Navigation.NavigationStack.GetType() != typeof(WaletServices))
            {
                await page.Navigation.PushAsync(new WaletServices(LoyaltySchemes, Services, Tasks,Orders));
            }
        }

        #endregion

        #region AccountData
        private Command getAccountCommand;

        public Command GetAccountCommand
        {
            get
            {
                return getAccountCommand ??
                    (getAccountCommand = new Command(async () => await ExecuteGetAccountCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetAccountCommand()
        {
            if (IsBusy)
                return;

            IsBusy = false;

            MenuItem mnu = new YomoneyApp.MenuItem();
            try
            {
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&MTI=0200";
                Body += "&ProcessingCode=310000";
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "Success" || response.ResponseCode == "00000")
                    {
                        Credit = response.Balance;
                        //Credit = Math.Round(decimal.Parse(response.Balance), 2).ToString() + " Credits";
                        Points = Math.Round((decimal)(response.Amount), 2).ToString() + " Points";
                    }
                    else
                    {
                        Credit = "--:--";
                        Points = "0.00 Points";
                    }

                }

            }
            catch (Exception ex)
            {
                Credit = "--.--";
                Points = "--.-- Points";
                accData = mnu;
            }

        }
        #endregion

        #region AdvertClicked
        //private Command getAdvertDetailCommand;
        public Command GetAdvertDetailCommand = new Command<MenuItem>(ExecuteGetAdvertDetailCommand);
        //public Command GetAdvertDetailCommand
        //{
        //    get
        //    {
        //        return getAdvertDetailCommand ??
        //            (getAdvertDetailCommand = new Command<MenuItem>(async () => await ExecuteGetAdvertDetailCommand(), () => { return !IsBusy; }));
        //    }
        //}

        private static void ExecuteGetAdvertDetailCommand(MenuItem mn)
        {


            // IsBusy = true;

            try
            {
                NavigationPage pag = new NavigationPage();
                ServiceViewModel svm = new ServiceViewModel(pag, mn);
                svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region CheckSpaniuser
        private Command checkUserCommand;

        public Command CheckUserCommand
        {
            get
            {
                return checkUserCommand ??
                    (checkUserCommand = new Command(async () => await ExecuteCheckUserCommand(), () => { return !IsBusy; }));
            }
        }

        //public MyTemplateSelector TemplateSelector { get; set; }

        private async Task ExecuteCheckUserCommand()
        {
            //  if (IsBusy)
            // return;

            IsBusy = true;
            showAlert = false;
            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = "330000";
                trn.Narrative = "Loyalty";
                trn.Note = "Redeem Points";
                trn.AgentCode = "0";
                trn.Quantity = 0;//redeemSection.Count;
                trn.ServiceProvider = "Redeem Points";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = 0;// redeemSection.ServiceId;
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
                Body += "&Mpin=" + trn.Mpin;
                Body += "&Note=" + trn.Note;


                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        await page.Navigation.PushAsync(new SpaniWorkSpace());
                    }
                    else if (response.ResponseCode == "12")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        ac.DeleteCredentials();
                        await page.Navigation.PushAsync(new AccountMain());

                    }
                    else
                    {
                        //await page.Navigation.PushAsync(new SpaniCreatePage());
                        await page.Navigation.PushAsync(new SpaniWorkSpace());
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                //GetRewardsListCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to open work space. Check your internet connectivity", "OK");

        }

        #endregion

        #region Object 

        string credit = string.Empty;
        public string Credit
        {
            get { return credit; }
            set { SetProperty(ref credit, value); }
        }

        string loyaltySchemes = string.Empty;
        public string LoyaltySchemes
        {
            get { return loyaltySchemes; }
            set { SetProperty(ref loyaltySchemes, value); }
        }

        string services = string.Empty;
        public string Services
        {
            get { return services; }
            set { SetProperty(ref services, value); }
        }

        string tasks = string.Empty;
        public string Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }
        string orders = string.Empty;
        public string Orders
        {
            get { return orders; }
            set { SetProperty(ref orders, value); }
        }
        string source = string.Empty;
        public string Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        string points = string.Empty;
        public string Points
        {
            get { return points; }
            set { SetProperty(ref points, value); }
        }

        bool showNav = true;
        public bool ShowNav
        {
            get { return showNav; }
            set { SetProperty(ref showNav, value); }
        }

        string routeId = string.Empty;
        public string RouteId
        {
            get { return routeId; }
            set { SetProperty(ref routeId, value); }
        }
                
        #endregion
    }
}

