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
using YomoneyApp.Services;
using YomoneyApp.Views.Login;
using YomoneyApp.Views.Profile;
using RetailKing.Models;
using YomoneyApp.Views.Webview;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Profile.Loyalty;
using YomoneyApp.Views;

namespace YomoneyApp
{
    public class WalletServicesViewModel : ViewModelBase
    {
        readonly IDataStore dataStore;
        string HostDomain = "http://192.168.100.150:5000";
        public ObservableRangeCollection<MenuItem> Stores { get; set; }
        public ObservableRangeCollection<Grouping<string, MenuItem>> StoresGrouped { get; set; }
        public ObservableRangeCollection<MenuItem> Categories { get; set; }
        public ObservableRangeCollection<MenuItem> Denominations { get; set; }
        public ObservableCollection<string> SubCategories { get; set; }
        public ObservableRangeCollection<MenuItem> Currencies { get; set; }
        public bool ForceSync { get; set; }

        public WalletServicesViewModel(Page page) : base(page)
        {
            Title = "My Money";
            dataStore = DependencyService.Get<IDataStore>();
            Stores = new ObservableRangeCollection<MenuItem>();
            StoresGrouped = new ObservableRangeCollection<Grouping<string, MenuItem>>();
            Categories = new ObservableRangeCollection<MenuItem>();
            Denominations = new ObservableRangeCollection<MenuItem>();
            Currencies = new ObservableRangeCollection<MenuItem>();
            SubCategories = new ObservableCollection<string>();
            String myDate = DateTime.Now.ToString();
            Datte = myDate;
        }

        public Action<MenuItem> ItemSelected { get; set; }

        MenuItem selectedStore;

        public MenuItem SelectedStore
        {
            get { return selectedStore; }
            set
            {
                selectedStore = value;
                OnPropertyChanged("SelectedStore");
                if (selectedStore == null)
                    return;

                if (ItemSelected == null)
                {
                    if (selectedStore.Title == "Sign Out")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        ac.DeleteCredentials();
                        page.Navigation.PushAsync(new AccountMain());
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else if (selectedStore.Title == "Account Topup")
                    {
                        page.Navigation.PushAsync(new TopupPage());
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else if (selectedStore.Title == "Profile")
                    {
                        page.Navigation.PushAsync(new TopupPage());
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else if (selectedStore.Title == "My Services" || selectedStore.Title == "Customer Services")
                    {
                        selectedStore.Title = "Customer Services";
                        page.Navigation.PushAsync(new ServiceProviders(SelectedStore));
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else if(selectedStore.Title == "YoApp Points")
                    {
                        page.Navigation.PushAsync(new LoyaltyRewards(SelectedStore));
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else if(selectedStore.Title == "My Tasks")
                    {
                        AccessSettings acnt = new AccessSettings();
                        string uname = acnt.UserName;
                        string link = "http://192.168.100.150:5000/Mobile/Projects?Id=" + uname;

                        page.Navigation.PushAsync(new WebviewHyubridConfirm(link, "My Tasks", true, "#df782d"));

                       //page.Navigation.PushModalAsync(new WebviewPage(link, "My Tasks", true, "#df782d"));
                        SelectedStore = null;
                        selectedStore = null;
                    }
                    else
                    {
                        page.Navigation.PushAsync(new SelectPage(selectedStore));
                        SelectedStore = null;
                       
                        selectedStore = null;
                    } 
                    
                }
                else
                {
                    ItemSelected.Invoke(selectedStore);
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
                Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                mnu.Add(new MenuItem { Title = "Loyalty Points", Image= "http://192.168.100.150:5000/Content/Spani/Images/Loyalty.jpg", Section = "Loyalty", ServiceId = 1, SupplierId = "All",TransactionType = 6 });
                mnu.Add(new MenuItem { Title = "My Services", Image = "http://192.168.100.150:5000/Content/Spani/Images/myServices.jpg", Section = "Yomoney", ServiceId = 11, SupplierId = "All", TransactionType = 1 });
                mnu.Add(new MenuItem { Title = "My Tasks", Image = "http://192.168.100.150:5000/Content/Spani/Images/tasks.jpg", Section = "Web", ServiceId = 1, SupplierId = "5-0001-0000000", TransactionType = 6, Description = "YoLifestyle" });
                mnu.Add(new MenuItem { Title = "Sign Out", Image = "http://192.168.100.150:5000/Content/Spani/Images/signOut.jpg", Section = "Yomoney", ServiceId = 5, SupplierId = "All", TransactionType = 1 });
                // var stores = await dataStore.GetStoresAsync();
                Stores.ReplaceRange(mnu);
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
                await page.DisplayAlert("Error!", "Unable to gather menus.", "OK");

        }

        #region Account Status
        private Command getAccountStatusCommand;

        public Command GetAccountStatusCommand
        {
            get
            {
                return getAccountStatusCommand ??
                    (getAccountStatusCommand = new Command(async () => await ExecuteGetAccountStatusCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetAccountStatusCommand()
        {
            if (IsBusy)
                return;

            Message = "Processing...";
            IsBusy = true;
            getAccountStatusCommand?.ChangeCanExecute();

            bool showAlert = false;
                        
            try
            {
                TransactionRequest trn = new TransactionRequest();
                AccessSettings accessSettings = new AccessSettings();

                trn.CustomerMSISDN = accessSettings.UserName;

                string Body = "";

                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/CheckAccountVerificationStatus/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        if (response.Note.ToUpper().Trim() == "TRUE")
                        {
                            AccountVerificationStatus = "VERIFIED";
                            VerificationColor = "#79c606";
                            IsNotVerified = false;
                        }
                        else
                        {
                            AccountVerificationStatus = "UNCONFIRMED";
                            VerificationColor = "#9b1003";
                            IsNotVerified = true;
                            StatusText = "If you verify your account, you will get the benefit of getting the verified status" +
                                " on the app and get deals from people who will be able to trust you";
                        }
                    }
                    else
                    {
                        AccountVerificationStatus = "UNCONFIRMED";
                        VerificationColor = "#9b1003";
                        IsNotVerified = true;
                        StatusText = "If you verify your account, you will get the benefit of getting the verified status" +
                                " on the app and get deals from people who will be able to trust you";
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
                getAccountStatusCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Error!", "There has been an error in loading your account status", "OK");

        }
        #endregion

        #region Submit Personal Details
        private Command submitPersonalDetailsCommand;

        public Command SubmitPersonalDetailsCommand
        {
            get
            {
                return submitPersonalDetailsCommand ??
                    (submitPersonalDetailsCommand = new Command(async () => await ExecuteSubmitPersonalDetailsCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteSubmitPersonalDetailsCommand()
        {
            TransactionResponse transactionResponse = new TransactionResponse();

            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(ActiveCountry))
            {
                await page.DisplayAlert("Error!", "Please select your Active Country.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                await page.DisplayAlert("Error!", "Please select your Gender.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Id))
            {
                await page.DisplayAlert("Error!", "Please enter your National ID Number.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Date.ToString()))
            {
                await page.DisplayAlert("Error!", "Please enter your DOB", "OK");
                return;
            }

            var ageInYears = GetDifferenceInYears(Date, DateTime.Today);

            if (ageInYears < 12)
            {
                await page.DisplayAlert("Age Error!", "You are too young to be using this app.", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            submitPersonalDetailsCommand?.ChangeCanExecute();
                      
            try
            {
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new AccessSettings();
                
                trn.Country = ActiveCountry;
                trn.CustomerAccount = Id;
                trn.Narrative = Gender;
                trn.Note = Date.ToString();
                trn.CustomerMSISDN = acnt.UserName;

                string Body = "";

                Body += "Country=" + trn.Country;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&Note=" + trn.Note;
                Body += "&CustomerMSISDN=" + trn.CustomerMSISDN;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/VerifyPersonalDetails/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result);
                   
                    if (transactionResponse.ResponseCode == "00000")
                    {
                        MessagingCenter.Send<string, string>("VerifyCustomer", "PersonalData", "Success");
                    }
                    else
                    {
                        MessagingCenter.Send<string, string>("VerifyCustomer", "PersonalData", transactionResponse.Description);
                    }

                }
                else
                {
                    MessagingCenter.Send<string, string>("VerifyCustomer", "PersonalData", "Failure");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
            finally
            {
                IsBusy = false;
                submitPersonalDetailsCommand.ChangeCanExecute();
            }            
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
                // trn.Product = Category;
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
                GetCurrenciesCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather Currencies.", "OK");


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
                trn.TransactionType = 7;
                trn.Note = "Currencies";
                // trn.Product = Category;
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
                Body += "&TransactionType=" + trn.TransactionType;
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

        #region GetPayment options
        public async Task<IEnumerable<MenuItem>> GetServicesAsync(MenuItem mnu)
        {
            if (IsBusy)
                return new List<MenuItem>();

            IsBusy = true;
            Message = "Loading payment options...";
            try
            {
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "3";
                trn.Note = mnu.Note;// "JobSectors";
                trn.Product = Category;
                trn.TransactionType = 3;
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
                    Categories.ReplaceRange(servics);
                    return servics;

                }
                return new List<MenuItem>();

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error!", "Unable to gather payment options.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }
        public async Task<IEnumerable<MenuItem>> GetPaymentsAsync()
        {
            List<MenuItem> payments = new List<YomoneyApp.MenuItem>();

            IsBusy = false;

            var b = GetBalanceAsync(); // get current account balance 
           
            var showAlert = false;

            IsBusy = true;
            Message = "Loading, please wait...";

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
                trn.Narrative = "8";
                trn.ServiceId = 14;
                trn.ServiceProvider = "Yomoney";
                trn.ServiceProvider = Address;
                trn.TransactionType = 8;
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
                    IsBusy = false;
                    Stores.Clear();
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Stores.ReplaceRange(servics);
                    return servics;
                }
                return payments;
            }
            catch (Exception ex)
            {
                //IsBusy = true;
                showAlert = true;
                return payments;
            }

        }
        public async Task<string> GetBalanceAsync()
        {
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

                        Balance = Math.Round(decimal.Parse(response.Balance), 2).ToString();

                        if(response.Narrative == "None")
                        {
                            ShowNavigation = true;
                        }
                    }
                    else
                    {
                        Balance = "$0.00";
                    }
                    
                }
                return Balance;
            }
            catch (Exception ex)
            {
                Balance = "$-.--";
            }
            return Balance;
        }
        #endregion

        #region  topup
        private Command getPaymentCommand;
        public Command GetPaymentCommand
        {
            get
            {
                return getPaymentCommand ??
                    (getPaymentCommand = new Command(async () => await ExecuteGetPaymentCommand(selectedStore), () => { return !IsBusy; }));
            }
        }
        private async Task ExecuteGetPaymentCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetPaymentCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

               // Stores.Clear();
               
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                trn.Narrative = "Account Topup";
                trn.Note = Category;
                trn.ServiceId = Stores.Where(u => u.Title == Category).FirstOrDefault().ServiceId;
                trn.Amount = decimal.Parse(Budget);
                trn.TransactionType = 8;
                if (PhoneNumber == null)
                {
                    trn.CustomerMSISDN = uname;
                }
                else
                {
                    trn.CustomerMSISDN = PhoneNumber;
                }
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
               
                client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000")
                    {
                        if (response.Description == "WebRedirect")
                        {
                            var servics = response.Narrative;
                            string source = HostDomain + "/Mobile/" + servics;
                            await page.Navigation.PushAsync(new WebviewHyubridConfirm(source, response.Note, true, null));
                        }
                        else
                        {
                            await page.DisplayAlert("Topup Success", "Credit topup was seccessful", "OK");
                        }
                    }
                    else
                    {
                        await page.DisplayAlert("Payment Error", response.Description, "OK");
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
                GetPaymentCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Payment Error!", "Sorry, please try again. Unexpected error ", "OK");

        }
        #endregion

        #region  payDirect
        private Command getPaymentDirectCommand;

        public Command GetPaymentDirectCommand
        {
            get
            {
                return getPaymentDirectCommand ??
                    (getPaymentDirectCommand = new Command(async () => await ExecuteGetPaymentDirectCommand(selectedStore), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetPaymentDirectCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            // if (ForceSync)
            //Settings.LastSync = DateTime.Now.AddDays(-30);

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter all fields", "OK");
                return;
            }

            IsBusy = true;
            Message = "Processing Payment...";

            var showAlert = false;
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
                trn.Narrative = "Service Payment";
                trn.Note = Category;
                trn.ServiceId = Stores.Where(u => u.Title == Category).FirstOrDefault().ServiceId;
                trn.Product = ServiceId.ToString();
                trn.Amount = decimal.Parse(Budget);
                trn.Currency = Currency;
                trn.TransactionType = 8;
                if (PhoneNumber == null)
                {
                    trn.CustomerMSISDN = uname;
                }
                else
                {
                    trn.CustomerMSISDN = PhoneNumber;
                }
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
                Body += "&Currency=" + trn.Currency;
                Body += "&TransactionType=" + trn.TransactionType;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "00000" || response.ResponseCode == "Success")
                    {
                        if (response.Description == "WebRedirect")
                        {
                            var servics = response.Narrative;
                            string source = HostDomain + "/Mobile/" + servics;
                            await page.Navigation.PushAsync(new WebviewHyubridConfirm(source, response.Note, false,null));
                        }
                        else
                        {
                            //await page.Navigation.PopAsync();
                            IsBusy = false;
                            MessagingCenter.Send<string, string>("PaymentRequest", "NotifyMsg", "Payment Success");
                            //await page.DisplayAlert("Topup Success", "Credit topup was seccessful", "OK");
                        }
                    }
                    else
                    {
                        IsBusy = false;
                        await page.DisplayAlert("Payment Error", response.Description, "OK");
                        MessagingCenter.Send<string, string>("PaymentRequest", "NotifyMsg", "Failed");
                    }
                }

            }
            catch (Exception ex)
            {
                IsBusy = false;
                showAlert = true;
                MessagingCenter.Send<string, string>("PaymentRequest", "NotifyMsg", "Failed");
            }
            finally
            {
                IsBusy = false;
                GetPaymentCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Payment Error", "Sorry please try again unexpected error ", "OK");

        }
        #endregion

        #region  Amount
        private Command getAmountCommand;

        public Command GetAmountCommand
        {
            get
            {
                return getAmountCommand ??
                    (getAmountCommand = new Command(async () => await ExecuteGetAmountCommand(selectedStore), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetAmountCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            getAmountCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
               if(Budget == null || Budget == "" || Budget == "0")
               {
                    IsBusy = false;
                    await page.DisplayAlert("Payment Error", "Please Enter a valid amount", "OK");   
               }
               else
               {
                    IsBusy = false;
                    MenuItem mn = new MenuItem();
                    mn.Amount = Budget;
                    mn.Currency = Currency;
                    GetPaymentCommand.ChangeCanExecute();
                    MessagingCenter.Send<string, MenuItem>("PaymentRequest", "GetAmountMsg", mn);

               }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                showAlert = true;
                if (showAlert)
                    await page.DisplayAlert("Payment Error:(", "Sorry please try again unexpected error ", "OK");
            }

        }
        #endregion

        #region Object 

        string gender = string.Empty;
        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value); }
        }

        string activeCountry = string.Empty;
        public string ActiveCountry
        {
            get { return activeCountry; }
            set { SetProperty(ref activeCountry, value); }
        }      

        string statusText = string.Empty;
        public string StatusText
        {
            get { return statusText; }
            set { SetProperty(ref statusText, value); }
        }

        bool isNotVerified = false;
        public bool IsNotVerified
        {
            get { return isNotVerified; }
            set { SetProperty(ref isNotVerified, value); }
        }

        string accountVerificationStatus = string.Empty;
        public string AccountVerificationStatus
        {
            get { return accountVerificationStatus; }
            set { SetProperty(ref accountVerificationStatus, value); }
        }
        string verificationColor = string.Empty;
        public string VerificationColor
        {
            get { return verificationColor; }
            set { SetProperty(ref verificationColor, value); }
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

        long serviceId =0;
        public long ServiceId
        {
            get { return serviceId; }
            set { SetProperty(ref serviceId, value); }
        }

        string budget = string.Empty;
        public string Budget
        {
            get { return budget; }
            set { SetProperty(ref budget, value); }
        }

        string balance = string.Empty;
        public string Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }

        string currency = string.Empty;
        public string Currency
        {
            get { return currency; }
            set { SetProperty(ref currency, value); }
        }


        string section = string.Empty;
        public string Section
        {
            get { return section; }
            set { SetProperty(ref section, value); }
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

        string id = string.Empty;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
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

        DateTime date = DateTime.Now ;
        public DateTime Date
        {
            get { return date; }
            set
            {
                SetProperty(ref date, value);
            }
        }

        public string Datte { get; }


        #endregion

        #region Age Difference
        int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year - 1) +
                (((endDate.Month > startDate.Month) ||
                ((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);
        }
        #endregion
    }

}

