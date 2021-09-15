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

namespace YomoneyApp
{
    public class HomeViewModel : ViewModelBase
    {
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
                    Latitude =  location.Latitude.ToString();
                    Longitude =  location.Longitude.ToString();
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
                    if(sh != null)
                    {
                       var e = DateTime.Parse(sh.ToString());
                        if(DateTime.Now.Subtract(e).Minutes > 30)
                        {
                            shownotify = true;
                        }

                    }
                    if (shownotify)
                    {
                        var notificator = DependencyService.Get<IToastNotificator>();

                        var options = new NotificationOptions()
                        {

                            Title = "Switch On Location",
                            Description = "To get deals near you on yomoney, switch on your location"
                        };

                        var result = notificator.Notify(options);

                        var resp = ac.SaveValue("LocationNotification", DateTime.Now.ToString()).Result;
                    }
                }
                catch (Exception n)
                {
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
                catch
                {

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
                /* mnu.Title = "Bill Payments";
                 mnu.Image = "payments_icon.png";
                 mnu.ServiceId = 11;
                 mnu.SupplierId = "All";
                 mnu.Section = "Yomoney";
                 mnu.TransactionType = 1;

                 myButtonSource.Add(mnu);*/
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
                await page.Navigation.PushAsync(new WaletServices());
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
                        Points  = Math.Round((decimal)(response.Amount), 2).ToString() + " Points";
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
                Credit  = "--.--";
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
                ServiceViewModel svm = new ServiceViewModel(pag,mn);
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
      
      

        #endregion
    }
}

