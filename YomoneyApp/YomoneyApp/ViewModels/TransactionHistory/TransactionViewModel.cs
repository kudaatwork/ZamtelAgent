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
using YomoneyApp.Views.Menus;
using YomoneyApp.Services;
using RetailKing.Models;
using System.IO;
using YomoneyApp.Views.Profile.Loyalty;
using YomoneyApp.Views.TransactionHistory;
using YomoneyApp.Views.Spend;
using YomoneyApp.Interfaces;

namespace YomoneyApp
{
    public class TransactionViewModel : ViewModelBase
    {
        string HostDomain = "https://www.ymoneyservice.com";
        string ProcessingCode = "350000";
        public MenuItem selec;
        readonly IDataStore dataStore;
        public bool ForceSync { get; set; }
        public ObservableRangeCollection<MenuItem> Transactions { get; set; }
        public ObservableRangeCollection<MenuItem> PendingTransactions { get; set; }
        public ObservableRangeCollection<MenuItem> Categories { get; set; }
      
        //public bool ForceSync { get; set; }
        public TransactionViewModel(Page page,MenuItem selected) : base(page)
        {
            Title = selected.Title;
            selec = selected;
            BannerImage =  "Transactions.jpg";
           
            dataStore = DependencyService.Get<IDataStore>();
            Transactions = new ObservableRangeCollection<MenuItem>();
            PendingTransactions = new ObservableRangeCollection<MenuItem>();
            Categories = new ObservableRangeCollection<MenuItem>();
            //GetSeledtedValues(selected);
        }

        public Action<MenuItem> ItemSelected { get; set; }

        MenuItem selectedTransaction;
        public MenuItem SelectedTransaction
        {
            get { return selectedTransaction; }
            set
            {
                selectedTransaction = value;
                OnPropertyChanged("SelectedTransaction");
                if (selectedTransaction == null)
                    return;

                if (ItemSelected == null)
                {
                    
                   if(selectedTransaction.Description != null)
                    {
                        try
                        {
                            #region SendToken 
                            var servics = new List<MenuItem>();
                            char[] delim = new char[] { '|' };
                            char[] delimeter = new char[] { '#' };
                            char[] delimeta = new char[] { '~' };
                            string[] tokens = selectedTransaction.Description.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                string[] img = selectedTransaction.Description.Split(delimeta, StringSplitOptions.RemoveEmptyEntries);
                                MenuItem mni = new MenuItem();
                                mni.Description = tokens[i];
                                if (img.Length > 1)
                                {
                                    mni.Description = img[0];
                                    mni.Image = img[img.Length - 1];
                                    
                                }
                            
                                servics.Add(mni);
                            }

                            #endregion
                            page.Navigation.PushAsync(new TockenPage(servics));
                        }catch
                        {
                            DependencyService.Get<IClipBoard>().OnCopy(selectedTransaction.Description);
                            //mess = "Data Copied";
                        }
                    }
                   
                    SelectedTransaction = null;  
                }
                else
                {
                    ItemSelected.Invoke(selectedTransaction);
                }
            }
        }

        MenuItem selectedRetry;
        public MenuItem SelectedRetry
        {
            get { return selectedRetry; }
            set
            {
                selectedRetry = value;
                OnPropertyChanged("SelectedRetry");

                if (selectedRetry == null)
                    return;
                if (ItemSelected == null)
                {

                    GetSuspensePickerAsync(selectedRetry);

                    SelectedRetry = null;
                    selectedRetry = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedRetry);
                }
            }
        }

        #region suspense retry
        public async void GetSuspensePickerAsync(MenuItem mn)
        {
            //if (IsBusy)
            //   return;
           // Message = "Processing please wait...";
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            //GetRetryTokenCommand.ChangeCanExecute();
            var showAlert = false;
            
            //string myinput = await PaymentCall(page.Navigation, "Payment");
            //MenuItem mn = new YomoneyApp.MenuItem();
            //mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
            //mn.Title = Category;

            //Message = "Processing request";
            #region process
            try
            {
                // if (ServiceOptions != null)
                //   ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = JsonConvert.DeserializeObject<TransactionRequest>(mn.Note);
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                /*trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                trn.Note = "Supplier";
                trn.TerminalId = "ClientApp";
                trn.TransactionRef = Email;
                trn.ServiceProvider = mn.SupplierId;
                trn.Narrative = "Bill Payment";
                trn.ServiceId = mn.ServiceId;//Categories.Where(u => u.Title == category).FirstOrDefault().TransactionType;
                trn.Product = mn.ActionId.ToString();// Categories.Where(u => u.Title == category).FirstOrDefault().Id;
                trn.ActionId = mn.ActionId;
                trn.Amount = decimal.Parse(mn.Amount);
                trn.CustomerMSISDN = AccountNumber;
                trn.Currency = mn.Currency ;
                trn.CustomerData = PhoneNumber;
                trn.OrderLines = Ptitle;
                trn.TransactionType = mn.TransactionType;*/
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
                        //Stores.ReplaceRange(servics);

                        Title = "Transaction Token";
                        //IsConfirm = false;
                       // Retry = false;
                        //Share = true;
                        await page.Navigation.PushAsync(new TockenPage(servics));
                    }
                    else
                    {
                        //IsConfirm = false;
                       // Retry = true;
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
                //GetTokenCommand.ChangeCanExecute();
            }
            #endregion

            //Message = "";
            if (showAlert)
                await page.DisplayAlert("Transaction Error", "The service timed out please retry to get response", "OK");

        }
        #endregion


        #region model

        DateTime date = DateTime.Now.Date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                SetProperty(ref date, value);
            }
        }
        DateTime enddate = DateTime.Now.Date;
        public DateTime EndDate
        {
            get { return enddate; }
            set
            {
                SetProperty(ref enddate, value);
            }
        }

        string category = string.Empty;
        public string Category
        {
            get { return category; }
            set { SetProperty(ref category, value); }
        }

        int categoryId = 1;
        public int CategoryId
        {
            get { return categoryId; }
            set { SetProperty(ref categoryId, value); }
        }

        int transactionType = 1;
        public int TransactionType
        {
            get { return transactionType; }
            set { SetProperty(ref transactionType, value); }
        }

        int currentPage = 1;
        public int CurrentPage
        {
            get { return currentPage; }
            set { SetProperty(ref currentPage, value); }
        }

        string account = string.Empty;
        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }

        string transactionData = string.Empty;
        public string TransactionData
        {
            get { return transactionData; }
            set { SetProperty(ref transactionData, value); }
        }

        string bannerImage = string.Empty;
        public string BannerImage
        {
            get { return bannerImage; }
            set { SetProperty(ref bannerImage, value); }
        }

        bool searchByAccount = false;
        public bool SearchByAccount
        {
            get { return searchByAccount; }
            set { SetProperty(ref searchByAccount, value); }
        }
        #endregion

        public void GetSeledtedValues(MenuItem mm)
        {
            selectedTransaction = mm;
            OnPropertyChanged("SelectedTransaction");
            if (selectedTransaction == null)
                return;

            if (ItemSelected == null)
            {

                GetHistoryCommand.Execute(null);
                SelectedTransaction = null;  
                
            }
            else
            {
                ItemSelected.Invoke(selectedTransaction);
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
                        MenuItem mm = new MenuItem();
                        mm.TransactionType = 0;
                        currentPage += 1;
                        IsBusy = false;
                        await ExecuteGetHistoryCommand(mm);
                    }));
            }
        }

        public async Task<IEnumerable<MenuItem>> GetHistoryPickerAsync(string note)
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
                trn.ProcessingCode = "420000";
                trn.Narrative = selec.TransactionType.ToString();
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
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather billers.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return new List<MenuItem>();
        }

        private Command getHistoryCommand;

        public Command GetHistoryCommand
        {
            get
            {
                return getHistoryCommand ??
                    (getHistoryCommand = new Command(async () => await ExecuteGetHistoryCommand(selectedTransaction), () => { return !IsBusy; }));
            }
        }

        private Command getTransactionCommand;

        public Command GetTransactionCommand
        {
            
            get
            {
                    return getTransactionCommand ??
                        (getTransactionCommand = new Command(async () => await ExecuteGetTransactionCommand(), () => { return !IsBusy; }));
                
            }
        }

        private async Task ExecuteGetTransactionCommand()
        {
            if (IsBusy)
                return;
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            if(date > enddate)
            {
                await page.DisplayAlert("Check Dates", "End Date can not be earlier than the Start Date.", "OK");
                return;
            }
            if(string.IsNullOrEmpty(category))
            {
                await page.DisplayAlert("Select Service", "Please select the service you want the transaction history for.", "OK");
                return;
            }

            IsBusy = true;
            GetTransactionCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                var p = Categories.Where(u => u.Title == category).FirstOrDefault();
                
                var ss = new MenuItem();
                ss.TransactionType = p.TransactionType;
                ss.Description = date.ToString("dd-MM-yyyy") + "~" + enddate.ToString("dd-MM-yyyy") + "~" + category + "~" + p.TransactionType;
                ss.SupplierId = Account;
                await page.Navigation.PushAsync(new HistoryList(ss));
            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;  

            }
            finally
            {
                IsBusy = false;
                GetTransactionCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather Transactions. Check your internet connection", "OK");

        }

        private async Task ExecuteGetHistoryCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetHistoryCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                //Transactions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                if (selec.TransactionType != 0)
                {

                    Account = selec.SupplierId;
                    category = selec.Section;
                    TransactionData = selec.Description;
                    TransactionType = (int)selec.TransactionType;
                }
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "440000";
                trn.Narrative = TransactionType.ToString();
                trn.TransactionRef = TransactionData;
                trn.CustomerData = Account;
                trn.ServiceProvider = category;
                trn.Quantity = currentPage;
                //var p = Categories.Where(u => u.Title == category).FirstOrDefault();
                // trn.TransactionRef = date.ToString() + ":" + enddate.ToString() + ":" + category + ":" + p.TransactionType;
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
                    if (servics.Count() == 0)
                    {
                        if (date == enddate)
                        {
                            await page.DisplayAlert("Transactions", "There are no records for this date :" + date, "OK");
                        }
                        else
                        {
                            await page.DisplayAlert("Transactions", "There are no records for the date range " + date  + "-" + enddate, "OK");
                        }
                    }
                    else
                    {
                        if (selec.TransactionType == 0)
                        {
                            Transactions.AddRange(servics);
                        }
                        else
                        {
                            Transactions.ReplaceRange(servics);
                        }

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
                GetHistoryCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather transactions. Check your internet connection", "OK");

        }

        #region check account 

        private Command getSearchCommand;

        public Command GetSearchCommand
        {

            get
            {
                return getSearchCommand ??
                    (getSearchCommand = new Command(async () => await ExecuteGetSearchCommand(), () => { return !IsBusy; }));

            }
        }

        private async Task ExecuteGetSearchCommand()
        {
            if (IsBusy)
                return;
            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            if (date > enddate)
            {
                await page.DisplayAlert("Check Dates", "End Date can not be earlier than the Start Date.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(category))
            {
                await page.DisplayAlert("Select Service", "Please select the service you want the transaction history for.", "OK");
                return;
            }

            IsBusy = true;
            GetSearchCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                var p = Categories.Where(u => u.Title == category).FirstOrDefault();

                var ss = new MenuItem();
                ss.TransactionType = p.TransactionType;
                ss.Description = date.ToString("dd-MM-yyyy") + "~" + enddate.ToString("dd-MM-yyyy") + "~" + category + "~" + p.TransactionType;
                ss.SupplierId = Account;
                await page.Navigation.PushAsync(new HistoryList(ss));
            }
            catch (Exception ex)
            {
                IsBusy = true;
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetSearchCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather Transactions. Check your internet connection", "OK");

        }


        #endregion 

        #region check pending 
        private Command getPendingCommand;

        public Command GetPendingCommand
        {
            get
            {
                return getPendingCommand ??
                    (getPendingCommand = new Command(async () => await ExecuteGetPendingCommand(selectedTransaction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetPendingCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            GetHistoryCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {

                //Transactions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                if (selec.TransactionType != 0)
                {

                    Account = selec.SupplierId;
                    category = selec.Section;
                    TransactionData = selec.Description;
                    TransactionType = (int)selec.TransactionType;
                }
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "440000";
                trn.Narrative = TransactionType.ToString();
                trn.TransactionRef = TransactionData;
                trn.CustomerData = Account;
                trn.ServiceProvider = "Timeout";
                trn.Quantity = currentPage;
                //var p = Categories.Where(u => u.Title == category).FirstOrDefault();
                // trn.TransactionRef = date.ToString() + ":" + enddate.ToString() + ":" + category + ":" + p.TransactionType;
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
                    if (servics.Count() == 0)
                    {
                        MenuItem mnui = new MenuItem();
                        mnui.Title = "Pending Transactions";
                        mnui.Description = "You have no pending transactions";
                        mnui.HasProducts = false;
                        PendingTransactions.Replace(mnui);
                    }
                    else
                    {
                        if (selec.TransactionType == 0)
                        {
                            PendingTransactions.AddRange(servics);
                        }
                        else
                        {
                            PendingTransactions.ReplaceRange(servics);
                        }

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
                GetPendingCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Service Error", "Unable to gather pending transactions. Check your internet connection", "OK");

        }

        #endregion

    }

}

