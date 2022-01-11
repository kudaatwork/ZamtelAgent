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
using YomoneyApp.Views.Spend;
using YomoneyApp.Services;
using YomoneyApp.Views.TransactionHistory;
using YomoneyApp.Views.Menus;
using YomoneyApp.Interfaces;
using YomoneyApp.Views.Services;
using Plugin.Share;
using Plugin.Share.Abstractions;
using YomoneyApp.Views.TemplatePages;
using ZXing.Net.Mobile.Forms;

namespace YomoneyApp
{
    public class SpendViewModel : ViewModelBase
    {
        string HostDomain = "http://192.168.100.150:5000";

        string ProcessingCode = "350000";
        string title = "";
        readonly IDataStore dataStore;
        public bool ForceSync { get; set; }
        public ObservableRangeCollection<MenuItem> Stores { get; set; }
        public ObservableRangeCollection<MenuItem> Categories { get; set; }
        public ObservableRangeCollection<MenuItem> Denominations { get; set; }
        public ObservableCollection<string> SubCategories { get; set; }

        private Func<string, Task<string>> _evaluateJavascript;
        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return _evaluateJavascript; }
            set { _evaluateJavascript = value; }
        }

        public SpendViewModel(Page page) : base(page)
        {
            Title = title;
            dataStore = DependencyService.Get<IDataStore>();
            Stores = new ObservableRangeCollection<MenuItem>();
            Categories = new ObservableRangeCollection<MenuItem>();
            Denominations = new ObservableRangeCollection<MenuItem>();
            SubCategories = new ObservableCollection<string>();
        }

        public Action<MenuItem> ItemSelected { get; set; }

        #region Share
        MenuItem selectedToCopy;

        public MenuItem SelectedToCopy
        {
            get { return selectedToCopy; }
            set
            {
                selectedToCopy = value;
                OnPropertyChanged("SelectedToCopy");
                if (selectedToCopy == null)
                    return;

                if (ItemSelected == null)
                {
                    if (CrossShare.Current.SupportsClipboard)
                    {
                        CrossShare.Current.Share(new ShareMessage
                        {
                            Title = selectedToCopy.Title,
                            Text = selectedToCopy.Description,
                            Url = ""
                        });
                    }
                    else
                    {
                        return;
                       // DependencyService.Get<IClipBoard>().OnCopy(selectedToCopy.Description);
                    }
                    Message = "Data Copied";
                    IsBusy = false;
                }
                else
                {
                    ItemSelected.Invoke(selectedToCopy);
                }
            }
        }

        public void GetShareToken(MenuItem mm)
        {
            //if (IsBusy)
            //   return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
           // GetRechargeCommand.ChangeCanExecute();
            if (CrossShare.Current.SupportsClipboard)
            {
                CrossShare.Current.Share(new ShareMessage
                {
                    Title = mm.Title,
                    Text = mm.Description,
                    Url = ""
                });
            }
            else
            {
                return;
                // DependencyService.Get<IClipBoard>().OnCopy(selectedToCopy.Description);
            }
            Message = "Data Copied";

        }    
        #endregion

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
                    title = selectedAction.Title.Trim();

                    if (selectedAction.Title.Trim() == "Pay Bill")
                    {
                        MenuItem mn = new MenuItem();
                        page.Navigation.PushAsync(new PayBill(mn));
                        SelectedAction = null;
                    }
                    else if (selectedAction.Title.Trim() == "Pin Airtime")
                    {

                        page.Navigation.PushAsync(new TockenRecharge(selectedAction));
                        SelectedAction = null;
                    }
                    else if (selectedAction.Title.Trim() == "Pinless Airtime")
                    {

                        page.Navigation.PushAsync(new Recharge(selectedAction));
                        SelectedAction = null;
                    }
                    else if (selectedAction.Title.Trim() == "Ticketing")
                    {
                        selectedAction.Title = "Services";
                        page.Navigation.PushAsync(new ServiceVariations(selectedAction));
                        SelectedAction = null;
                    }
                    else if (selectedAction.Title.Trim() == "Transaction History")
                    {
                        page.Navigation.PushAsync(new SearchByAccount());
                        SelectedAction = null;
                    }
                    else if (selectedAction.Title.Trim() == "Other Services")
                    {
                        page.Navigation.PushAsync(new SelectPage(selectedAction));
                        SelectedAction = null;
                    }
                }
                else
                {
                    ItemSelected.Invoke(selectedAction);
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
                trn.TransactionType = 7;

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
                await page.DisplayAlert("Error!", "Unable to gather stores.", "OK");

        }

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
                trn.TransactionType = 7;
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
                    SubCategories.Clear();
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
                await page.DisplayAlert("Error!", "Unable to gather " + Category + " services.Check your internet connection", "OK");


        }

        public async Task<IEnumerable<MenuItem>> GetStoreAsync(string note)
        {
            if (IsBusy)
                return new List<MenuItem>();

            Message = "loading billers...";
            IsBusy = true;
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
                trn.Note = note;// "JobSectors";
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

                    // Filter DSTV Payment out of the picture
                    foreach (var item in servics)
                    {
                        if (item.Title == "DSTV PAYMENT" || item.Title.Contains("DSTV"))
                        {
                            servics.Remove(item);
                            break;
                        }                        
                    }                   
                    
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

        public async Task<IEnumerable<MenuItem>> GetTopupAsync(string note, MenuItem selected)
        {
            if (IsBusy)
                return Categories;
            Message = "loading service providers...";
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

        #region Spend Menu

        private Command getMenuCommand;

        public Command GetMenuCommand
        {
            get
            {
                return getMenuCommand ??
                    (getMenuCommand = new Command(async () => await ExecuteGetMenuCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetMenuCommand()
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
                mnu.Add(new MenuItem { Title = "Pay Bill", Description="Pay for your utility bill", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 11, SupplierId = "All", TransactionType = 1 });
                mnu.Add(new MenuItem { Title = "Pinless Airtime", Description = "Recharge your airtime or internet direct to phone or account", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 2 });
                mnu.Add(new MenuItem { Title = "Pin Airtime", Description = "Get recharge pin for airtime, wifi or internet", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 8, SupplierId = "All", TransactionType = 2 });
                //mnu.Add(new MenuItem { Title = "Ticketing", Description = "Get recharge pin for airtime, wifi & internet", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 22, SupplierId = "All", TransactionType = 12});
                // mnu.Add(new MenuItem { Title = "More Services", Image = "Paymenu.png", Section = "Supplier Services", ServiceId = 12, SupplierId = "All", TransactionType = 1 });
                mnu.Add(new MenuItem { Title = "Transaction History", Description = "View your spending history", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 3 });
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
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather loyalty options.", "OK");

        }

        private Command getTopupMenuCommand;

        public Command GetTopupMenuCommand
        {
            get
            {
                return getTopupMenuCommand ??
                    (getTopupMenuCommand = new Command(async () => await ExecuteGetTopupMenuCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetTopupMenuCommand()
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
                mnu.Add(new MenuItem { Title = "Pinless Airtime", Image = "workMenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 2 });
                mnu.Add(new MenuItem { Title = "Token Airtime", Image = "workMenu.png", Section = "Yomoney", ServiceId = 8, SupplierId = "All", TransactionType = 2 });
                mnu.Add(new MenuItem { Title = "Transaction History", Image = "workMenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 4 });
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
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather options.", "OK");

        }

        #endregion

        #region GetToken
       

        #region Retry
        private Command getRetryTokenCommand;

        public Command GetRetryTokenCommand
        {
            get
            {
                return getRetryTokenCommand ??
                    (getTokenCommand = new Command(async () => await ExecuteGetRetryTokenCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetRetryTokenCommand()
        {
            //if (IsBusy)
            //   return;
            Message = "Processing please wait...";
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetRetryTokenCommand.ChangeCanExecute();
            var showAlert = false;
            if(sendSms == false)
            {
                PhoneNumber = "";
            }
            //string myinput = await PaymentCall(page.Navigation, "Payment");
            MenuItem mn = new YomoneyApp.MenuItem();
            mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
            mn.Title = Category;
          
               // Message = "Processing request";
                #region process
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
                    trn.ProcessingCode = "320000";
                    trn.Note = "Supplier";
                    trn.TerminalId = "ClientApp";
                    trn.TransactionRef = Email;
                    trn.ServiceProvider = "Bill Payment";
                    trn.Narrative = "Bill Payment";
                    trn.ServiceId = ServiceId;//Categories.Where(u => u.Title == category).FirstOrDefault().TransactionType;
                    trn.Product = JobPostId;// Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                    trn.Amount = decimal.Parse(Budget);
                    trn.CustomerMSISDN = AccountNumber;
                    trn.Currency = currency;
                    trn.CustomerData = PhoneNumber;
                    trn.OrderLines = Ptitle;
                    trn.TransactionType = 7;
                    string Body = "";
                    Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                    Body += "&CustomerAccount=" + trn.CustomerAccount;
                    Body += "&AgentCode=" + trn.AgentCode;
                    Body += "&Action=Mobile";
                    Body += "&OrderLines=" + trn.OrderLines;
                    Body += "&TerminalId=" + trn.TerminalId;
                    Body += "&TransactionRef=" + trn.TransactionRef;
                    Body += "&ServiceId=" + trn.ServiceId;
                    Body += "&Product=" + trn.Product;
                    Body += "&Amount=" + trn.Amount;
                    Body += "&MTI=" + trn.MTI;
                    Body += "&ProcessingCode=" + trn.ProcessingCode;
                    Body += "&Currency=" + trn.Currency;
                    Body += "&ServiceProvider=" + trn.ServiceProvider;
                    Body += "&Narrative=" + trn.Narrative;
                    Body += "&CustomerData=" + trn.CustomerData;
                    Body += "&Quantity=" + trn.Quantity;
                    Body += "&Note=" + trn.Note;
                    Body += "&Mpin=" + trn.Mpin;
                    Body += "&TransactionType=" + trn.TransactionType;

                    HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(180);
                    var myContent = Body;
                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);
                    if (result != "System.IO.MemoryStream")
                    {

                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        if (response.ResponseCode == "Success" || response.ResponseCode == "00000")
                        {
                            #region SendToken 
                            var servics = new List<MenuItem>();
                            char[] delim = new char[] { '|' };
                            char[] delimeter = new char[] { '#' };
                            string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                MenuItem mni = new MenuItem();
                                mni.Description = tokens[i];
                                servics.Add(mni);
                            }

                            #endregion
                            Stores.ReplaceRange(servics);

                            Title = "Transaction Token";
                            IsConfirm = false;
                            Retry = false;
                            Share = true;
                            await page.Navigation.PushAsync(new TockenPage(servics));
                        }
                        else
                        {
                            IsConfirm = false;
                            Retry = true;
                            await page.DisplayAlert("Transaction Error", response.Description + " please try again ", "OK");
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
                    GetTokenCommand.ChangeCanExecute();
                }
                #endregion

                Message = "";
                if (showAlert)
                    await page.DisplayAlert("Transaction Error", "The service timed out please retry to get response", "OK");
                

        }
        #endregion

        private Command getTokenCommand;

        public Command GetTokenCommand
        {
            get
            {
                return getTokenCommand ??
                    (getTokenCommand = new Command(async () => await ExecuteGetTokenCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetTokenCommand()
        {
            //if (IsBusy)
            //   return;
            Message = "Processing please wait...";
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetTokenCommand.ChangeCanExecute();
            var showAlert = false;
            if (sendSms == false)
            {
                PhoneNumber = "";
            }
            //string myinput = await PaymentCall(page.Navigation, "Payment");
            MenuItem mn = new YomoneyApp.MenuItem();
            mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
            mn.Title = Category;
            mn.ServiceId = ServiceId;
            await page.Navigation.PushAsync(new PaymentPage(mn));
            //IsBusy = false;
            MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (sender, arg) =>
            {

                if (arg == "Payment Success")
                {
                    IsBusy = true;
                    Message = "Processing request";
                    #region process
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
                        trn.ProcessingCode = "320000";
                        trn.Note = "Supplier";
                        trn.TerminalId = "ClientApp";
                        trn.TransactionRef = Email;
                        trn.ServiceProvider = "Bill Payment";
                        trn.Narrative = "Bill Payment";
                        trn.ServiceId = ServiceId;//Categories.Where(u => u.Title == category).FirstOrDefault().TransactionType;
                        trn.Product = JobPostId;// Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                        trn.Amount = decimal.Parse(Budget);
                        trn.CustomerMSISDN = AccountNumber;
                        trn.CustomerData = PhoneNumber;
                        trn.OrderLines = Ptitle;
                        trn.TransactionType = 7;
                        trn.Currency = mn.Currency;
                        string Body = "";
                        Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                        Body += "&CustomerAccount=" + trn.CustomerAccount;
                        Body += "&AgentCode=" + trn.AgentCode;
                        Body += "&Action=Mobile";
                        Body += "&OrderLines=" + trn.OrderLines;
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
                        Body += "&Currency=" + trn.Currency;
                        Body += "&Mpin=" + trn.Mpin;
                        Body += "&TransactionType=" + trn.TransactionType;

                        HttpClient client = new HttpClient();
                        client.Timeout = TimeSpan.FromSeconds(180);
                        var myContent = Body;
                        string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                        string result = await client.GetStringAsync(paramlocal);
                        if (result != "System.IO.MemoryStream")
                        {

                            var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                            if (response.ResponseCode == "Success" || response.ResponseCode == "00000")
                            {
                                #region SendToken 
                                var servics = new List<MenuItem>();
                                char[] delim = new char[] { '|' };
                                char[] delimeter = new char[] { '#' };
                                char[] delimeta = new char[] { '~' };
                                string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < tokens.Length; i++)
                                {
                                    string[] img = response.TransactionCode.Split(delimeta, StringSplitOptions.RemoveEmptyEntries);
                                    MenuItem mni = new MenuItem();
                                    mni.Description = tokens[i];
                                    if(img.Length > 1)
                                    {
                                        mni.Description = img[0];
                                        mni.Image = img[img.Length - 1];
                                    }
                                   // mni.Note = tokens[i];
                                    servics.Add(mni);
                                }

                                #endregion

                                Stores.ReplaceRange(servics);

                                Title = "Transaction Token";
                                IsConfirm = false;
                                Retry = false;
                                Share = true;
                                await page.Navigation.PushAsync(new TockenPage(servics));
                            }
                            else
                            {
                                IsConfirm = false;
                                Retry = true;
                                await page.DisplayAlert("Transaction Error", response.Description + " please try again ", "OK");
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
                        GetTokenCommand.ChangeCanExecute();
                    }
                    #endregion

                    Message = "";
                    if (showAlert)
                        await page.DisplayAlert("Transaction Error", "The service timed out please retry to get response", "OK");
                }
                else
                {
                    IsBusy = false;
                    //await page.DisplayAlert("Payment Failed", "Payment Failed", "OK");
                    await page.Navigation.PopAsync();
                    GetTokenCommand.ChangeCanExecute();
                }
                MessagingCenter.Unsubscribe<string, string>("PaymentRequest", "NotifyMsg");
            });

        }
        #endregion

        #region GetRecharge

        private Command getRechargeCommand;

        public Command GetRechargeCommand
        {
            get
            {
                return getRechargeCommand ??
                    (getRechargeCommand = new Command(async () => await ExecuteGetRechargeCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetRechargeCommand()
        {
            if (IsBusy)
                return;

            // if (ForceSync)
            //Settings.LastSync = DateTime.Now.AddDays(-30);

            if (string.IsNullOrWhiteSpace(AccountNumber) || string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Enter All Fields", "Please enter all fields", "OK");
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
            mn.Title = Category;
          
                await page.Navigation.PushAsync(new PaymentPage(mn));

                MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (sender, arg) =>
                {
                    if (arg == "Payment Success")
                    {
                        IsBusy = true;
                        Message = "Processing " + mn.Title;
                        #region getrecharge
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
                            trn.ProcessingCode = "320000";
                            trn.Narrative = "7";
                            trn.Note = "Yomoney";
                            trn.CustomerData = PhoneNumber;
                            trn.TerminalId = "ClientApp";
                            trn.TransactionRef = "";
                            trn.TransactionType = 2;
                            trn.ServiceProvider = "Purchase";
                            trn.Narrative = "Purchase";
                            trn.Note = "YomoneySupplier";
                            trn.ServiceId = Categories.Where(u => u.Title == category).FirstOrDefault().ServiceId;
                            trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                            trn.ActionId = long.Parse(Categories.Where(u => u.Title == category).FirstOrDefault().Id);
                            trn.Amount = decimal.Parse(Budget);
                            trn.Currency = mn.Currency;
                            trn.CustomerMSISDN = AccountNumber;
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
                            Body += "&TransactionType=" + trn.TransactionType ;
                            Body += "&ActionId=" + trn.ActionId;


                            HttpClient client = new HttpClient();
                            client.Timeout = TimeSpan.FromSeconds(180);
                            var myContent = Body;
                            string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                            string result = await client.GetStringAsync(paramlocal);
                            if (result != "System.IO.MemoryStream")
                            {

                                var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                                if (response.ResponseCode == "Success" || response.ResponseCode == "00000" || response.ResponseCode == "11304")
                                {
                                    #region SendToken
                                    if (response.TransactionCode == null && response.CustomerData != null)
                                    {
                                        response.TransactionCode = response.CustomerData;
                                    }
                                    else if (response.TransactionCode == null)
                                    {
                                        response.TransactionCode = "You Have successfully recharged $" + trn.Amount + " delivered with love ";
                                    }
                                    var servics = new List<MenuItem>();
                                    char[] delim = new char[] { '|' };
                                    char[] delimeter = new char[] { '#' };
                                    string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);

                                    if (tokens.Count() > 0)
                                    {
                                        for (int i = 0; i < tokens.Length; i++)
                                        {
                                            MenuItem mni = new MenuItem();
                                            mni.Description = tokens[i];
                                            servics.Add(mni);
                                        }
                                    }else
                                    {
                                        MenuItem mni = new MenuItem();
                                        mni.Description = response.TransactionCode;
                                        servics.Add(mni);
                                    }

                                  #endregion
                                    Retry = false;
                                    await page.Navigation.PushAsync(new TockenPage(servics));
                                    Stores.ReplaceRange(servics);
                                }
                                else
                                {

                                    Retry = true;
                                    IsConfirm = false;
                                    await page.DisplayAlert("Recharge Error", response.Description, "OK");
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
                            GetRechargeCommand.ChangeCanExecute();
                        }

                        if (showAlert)
                            await page.DisplayAlert("Service TimedOut", "The service timed out Check your Logs for the response", "OK");
                    #endregion
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

        private Command getRetryRechargeCommand;

        public Command GetRetryRechargeCommand
        {
            get
            {
                return getRetryRechargeCommand ??
                    (getRetryRechargeCommand = new Command(async () => await ExecuteGetRetryRechargeCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetRetryRechargeCommand()
        {
            //if (IsBusy)
            //   return;

            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            Message = "Processing, please wait....";

            var showAlert = false;
            if (sendSms == false)
            {
                PhoneNumber = "";
            }
                MenuItem mn = new YomoneyApp.MenuItem();
                mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
                mn.Title = Category;
            
                IsBusy = true;
                Message = "Processing " + mn.Title;
                #region getrecharge
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
                    trn.ProcessingCode = "320000";
                    trn.Note = "Yomoney";
                    trn.CustomerData = PhoneNumber;
                    trn.TerminalId = "ClientApp";
                    trn.TransactionRef = "retry";
                    trn.TransactionType = 7;
                    trn.ServiceProvider = "Purchase";
                    trn.Narrative = "Purchase";
                    trn.Note = "YomoneySupplier";
                    trn.ServiceId = Categories.Where(u => u.Title == category).FirstOrDefault().ServiceId;
                    trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                    trn.Amount = decimal.Parse(Budget);
                    trn.CustomerMSISDN = AccountNumber;
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
                    client.Timeout = TimeSpan.FromSeconds(180);
                    var myContent = Body;
                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);
                    if (result != "System.IO.MemoryStream")
                    {

                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        if (response.ResponseCode == "Success" || response.ResponseCode == "00000" || (response.ResponseCode == "11304" && response.CustomerData != null))
                        {
                            #region SendToken
                            if (response.TransactionCode == null && response.CustomerData != null)
                            {
                                response.TransactionCode = response.CustomerData;
                            }
                            else if (response.TransactionCode == null)
                            {
                                response.TransactionCode = "You Have successfully recharged $" + trn.Amount + " delivered with love ";
                            }
                            var servics = new List<MenuItem>();
                            char[] delim = new char[] { '|' };
                            char[] delimeter = new char[] { '#' };
                            string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);

                            if (tokens.Count() > 0)
                            {
                                for (int i = 0; i < tokens.Length; i++)
                                {
                                    MenuItem mni = new MenuItem();
                                    mni.Description = tokens[i];
                                    servics.Add(mni);
                                }
                            }
                            else
                            {
                                MenuItem mni = new MenuItem();
                                mni.Description = response.TransactionCode;
                                servics.Add(mni);
                            }

                        #endregion
                            Retry = false;
                            await page.Navigation.PushAsync(new TockenPage(servics));
                            Stores.ReplaceRange(servics);
                        }
                        else
                        {

                            Retry = true;
                            IsConfirm = false;
                            await page.DisplayAlert("Error!", response.Description, "OK");
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
                    GetRechargeCommand.ChangeCanExecute();
                }

                if (showAlert)
                    await page.DisplayAlert("Service TimedOut", "The service timed out Check your Logs for the response", "OK");
                #endregion
            
        }
        #endregion

        #region GetProductsVariations

        public async Task<IEnumerable<MenuItem>> GetProductsAsync()
        {
            if (IsBusy)
                return new List<MenuItem>();

            IsBusy = true;
            try
            {
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                var selected = Categories.Where(u => u.Title == category).FirstOrDefault();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "450000";
                trn.Narrative = selected.TransactionType.ToString();
                trn.TransactionType = selected.TransactionType;
                trn.ServiceId = selected.ServiceId;
                trn.AgentCode = selected.SupplierId;
                trn.Note = selected.Section;// "Yomoney";// "JobSectors";
                trn.Product = selected.Id;
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
                    Denominations.ReplaceRange(servics);
                    return servics;

                }
                return new List<MenuItem>();

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error!", "Unable to gather Packages.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }
        #endregion

        #region check Account

        Command checkAccountCommand;
        public Command CheckAccountCommand
        {
            get
            {
                return checkAccountCommand ??
                    (checkAccountCommand = new Command(async () => await ExecuteCheckAccountCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteCheckAccountCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Budget))
            {
                await page.DisplayAlert("Enter Amount", "Please enter a amount for the payment .", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(AccountNumber) && RequireAccount == true)
            {
                await page.DisplayAlert("Enter Account", "Please enter the billing account.", "OK");
                return;
            }


            IsBusy = true;

            try
            {
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0200";
                trn.ProcessingCode = "300000";
                trn.Narrative = Description;
                trn.Note = "Bill Payment";
                if (Categories.Count > 0)
                {
                    var biller = Categories.Where(u => u.Title == category).FirstOrDefault();
                    trn.ServiceId = biller.ServiceId;
                    trn.Product = biller.Id;
                    trn.AgentCode = biller.SupplierId;
                    trn.Source = biller.Section;
                    if (!string.IsNullOrEmpty(biller.WebLink))
                    {
                        trn.Note = "Web";
                        trn.Narrative = biller.WebLink;
                    }
                }
                else
                {
                    trn.ServiceId = long.Parse(category);
                    trn.Product = jobPostId;
                }
                trn.Amount = decimal.Parse(Budget);
                trn.CustomerMSISDN = AccountNumber;
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
                Body += "&Source=" + trn.Source;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                if (trn.Note != "Web")
                {
                    var client = new HttpClient();
                    var myContent = Body;
                    string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                    string result = await client.GetStringAsync(paramlocal);
                    if (result != "System.IO.MemoryStream")
                    {
                        var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                        if (response.ResponseCode == "00000")
                        {
                            trn.CustomerData = response.CustomerData;
                            trn.Source = PhoneNumber;
                            trn.ServiceProvider = Email;
                            trn.Narrative = category;
                            trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                            if (Retry)
                            {
                                trn.Note = "Reward Service";
                            }

                            await page.Navigation.PushAsync(new ConfirmPage(trn));
                            //var answer = await page.DisplayAlert("Confirm Details", response.CustomerData, "Yes", "No");
                            //if (answer == true)
                            //{
                            //   GetTokenCommand.Execute(null);
                            //}
                        }
                        else if (response.ResponseCode == "Web")
                        {
                            trn.CustomerData = response.CustomerData;
                            trn.Source = PhoneNumber;
                            trn.ServiceProvider = Email;
                            trn.Narrative = category;
                            trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                            if (Retry)
                            {
                                trn.Note = "Reward Service";
                            }

                            await page.Navigation.PushAsync(new WebConfirmPage(response.CustomerData, "Confirm Page", false, trn));

                        }
                        else
                        {
                            await page.DisplayAlert("Transaction Error", response.Description, "OK");
                        }
                    }
                }
                else
                {
                    //trn.CustomerData =  string.Format(HostDomain + "/Mobile/WebPartView/?{0}", Body);
                    string webUrl = trn.Narrative;
                    trn.Source = PhoneNumber;
                    trn.ServiceProvider = Email;
                    trn.Narrative = category;
                    trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                    if (Retry)
                    {
                        trn.Note = "Reward Service";
                    }

                    await page.Navigation.PushAsync(new WebConfirmPage(webUrl, "Confirm Page", false, trn));

                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Transaction Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                checkAccountCommand?.ChangeCanExecute();
            }

            ///await page.Navigation.PopAsync();

        }
        #endregion
     
        #region MasterPay
        public void GetMasterPay(MenuItem mm)
        {
            selectedAction = mm;
            OnPropertyChanged("selectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                MasterPayCommand.Execute(null);
                /////selectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            }
        }

        private Command masterPayCommand;

        public Command MasterPayCommand
        {
            get
            {
                return masterPayCommand ??
                    (masterPayCommand = new Command(async () => await ExecuteMasterPayCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteMasterPayCommand()
        {
            //if (IsBusy)
            //   return;
            Message = "Processing please wait...";
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetTokenCommand.ChangeCanExecute();
            var showAlert = false;
            if (sendSms == false)
            {
                PhoneNumber = "";
            }
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
                trn.ProcessingCode = "320000";
                trn.Narrative = "7";
                trn.TransactionType = 5;
                trn.Note = "Yomoney";
                trn.TerminalId = "ClientApp";
                trn.TransactionRef = "";
                trn.ServiceProvider = "Payment";
                trn.Narrative = "Payment";
                trn.Note = "CreateBid";
                trn.ServiceId = 16;
                //trn.Product = Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                trn.Amount = decimal.Parse(Budget);
                trn.CustomerMSISDN = AccountNumber;
                trn.CustomerData = PhoneNumber;
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
                client.Timeout = TimeSpan.FromSeconds(180);
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {

                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.ResponseCode == "Success" || response.ResponseCode == "00000")
                    {
                        #region SendToken 
                        var servics = new List<MenuItem>();
                        char[] delim = new char[] { '|' };
                        char[] delimeter = new char[] { '#' };
                        string[] tokens = response.TransactionCode.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < tokens.Length; i++)
                        {
                            MenuItem mni = new MenuItem();
                            mni.Description = tokens[i];
                            servics.Add(mni);
                        }

                        #endregion
                        await page.Navigation.PushAsync(new TockenPage(servics));
                        Stores.ReplaceRange(servics);
                    }
                    else
                    {
                        await page.DisplayAlert("Error!", response.Description, "OK");
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
                GetTokenCommand.ChangeCanExecute();
            }
            Message = "";
            if (showAlert)
                await page.DisplayAlert("Service TimedOut", "The service timed out Check your Logs for the response", "OK");


        }
        #endregion

        #region Object 
        bool error404 = false;
        public bool Error404
        {
            get { return error404; }
            set { SetProperty(ref error404, value); }
        }
              
        bool share = false;
        public bool Share 
        {
            get { return share; }
            set { SetProperty(ref share, value); }
        }

        bool requireAccount = false;
        public bool RequireAccount
        {
            get { return requireAccount; }
            set { SetProperty(ref requireAccount, value); }
        }

        bool isConfirmed = false;
        public bool IsConfirmed
        {
            get { return isConfirmed; }
            set { SetProperty(ref isConfirmed, value); }
        }

        bool retry = false;
        public bool Retry
        {
            get { return retry ; }
            set { SetProperty(ref retry, value); }
        }

        string retryText = string.Empty;
        public string RetryText
        {
            get { return retryText; }
            set { SetProperty(ref retryText, value); }
        }

        string budget = string.Empty;
        public string Budget
        {
            get { return budget; }
            set { SetProperty(ref budget, value); }
        }
        string currency = "ZWL";
        public string Currency
        {
            get { return currency; }
            set { SetProperty(ref currency, value); }
        }

        string accountNumber = string.Empty;
        public string AccountNumber
        {
            get { return accountNumber; }
            set { SetProperty(ref accountNumber, value); }
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

        bool isConfirm = true;
        public bool IsConfirm
        {
            get { return isConfirm; }
            set { SetProperty(ref isConfirm, value); }
        }

        bool hasProducts = false;
        public bool HasProducts
        {
            get { return hasProducts; }
            set { SetProperty(ref hasProducts, value); }
        }

        bool sendSms = false;
        public bool SendSms
        {
            get { return sendSms; }
            set { SetProperty(ref sendSms, value); }
        }

        bool sendEmail = false;
        public bool SendEmail
        {
            get { return sendEmail; }
            set { SetProperty(ref sendEmail, value); }
        }

        string email = string.Empty;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
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
            get { return category; }
            set { SetProperty(ref category, value); }
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

        long serviceId = 10;
        public long ServiceId
        {
            get { return serviceId; }
            set
            {
                SetProperty(ref serviceId, value);
            }
        }


        #endregion

    }
}

