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
using YomoneyApp.Views.Spend;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Webview;

namespace YomoneyApp
{
    public class LoyaltyViewModel : ViewModelBase
    {
        string HostDomain = "http://192.168.100.150:5000";
        string ProcessingCode = "350000";
        readonly IDataStore dataStore;
        public ObservableRangeCollection<MenuItem> Stores { get; set; }
        public ObservableRangeCollection<Grouping<string, MenuItem>> LoyaltyGrouped { get; set; }
        public bool ForceSync { get; set; }
        public LoyaltyViewModel(Page page, MenuItem selected) : base(page)
        {
            Title = selected.Title;
            Stores = new ObservableRangeCollection<MenuItem>();
        }
        public Action<MenuItem> ItemSelected { get; set; }

        MenuItem selectedSection;
        public MenuItem SelectedSection
        {
            get { return selectedSection; }
            set
            {
                selectedSection = value;
                OnPropertyChanged("SelectedSection");
                if (selectedSection == null)
                    return;

                if (ItemSelected == null)
                {
                    if (selectedSection.Title == "Redeem Rewards")
                    {
                        page.Navigation.PushAsync(new LoyaltyRewards(selectedSection));
                        SelectedSection = null;
                    }
                    else if (selectedSection.Title == "Transaction History")
                    {
                        page.Navigation.PushAsync(new HistoryRequestPage(selectedSection));
                        SelectedSection = null;
                    }
                    else if(selectedSection.ServiceId == 2 )
                    {
                        RewardAlerts();
                        SelectedSection = null;
                    }
                    
                }
                else
                {
                    ItemSelected.Invoke(selectedSection);
                }
            }
        }

        MenuItem redeemSection;
        public MenuItem RedeemSection
        {
            get { return redeemSection; }
            set
            {
                redeemSection = value;
                OnPropertyChanged("RedeemSection");
                if (redeemSection == null)
                    return;

                if (ItemSelected == null)
                {
                    supplierId = redeemSection.SupplierId;
                    quantity = redeemSection.Count;
                    serviceId = long.Parse(redeemSection.Id);
                       RewardAlerts();
                        RedeemSection = null;  
                }
                else
                {
                    ItemSelected.Invoke(redeemSection);
                }
            }
        }

        private Command forceRefreshCommand;

        public Command ForceLoyalRefreshCommand
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

        #region model
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

        #endregion

        #region LoyalActions
        public void GetSelectedSupplier(MenuItem mm)
        {
            selectedSection = mm;
            OnPropertyChanged("SelectedSection");
            if (selectedSection == null)
                return;

            if (ItemSelected == null)
            {
                Supplier = mm.Title;
                Points = mm.Count;
                GetStoresCommand.Execute(null);
                SelectedSection = null;
            }
            else
            {
                ItemSelected.Invoke(selectedSection);
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
                points = selectedSection.Count;
                List<MenuItem> mnu = new List<MenuItem>();
                mnu.Add(new MenuItem { Title = "Redeem Rewards", Image = selectedSection.Image, Section = "Yomoney", ServiceId = 1, SupplierId = selectedSection.SupplierId, TransactionType = 7, Count= selectedSection.Count,Description = selectedSection.Description });
                mnu.Add(new MenuItem { Title = "Transaction History", Image = selectedSection.Image, Section = "Yomoney", ServiceId = 1, SupplierId = selectedSection.SupplierId, TransactionType = 6, Count = selectedSection.Count, Description = selectedSection.Description });
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

        #endregion

        #region LoyalRewards
         public void GetSelectedRewards(MenuItem mm)
         {
           selectedSection = mm;
           OnPropertyChanged("SelectedSection");
           if (selectedSection == null)
             return;

           if (ItemSelected == null)
           {

                GetRewardsListCommand.Execute(null);
                SelectedSection = null;
           }
           else
           {
             ItemSelected.Invoke(selectedSection);
           }
     }
   
         private Command getRewardsListCommand;

         public Command GetRewardsListCommand
         {
             get
             {
                 return getRewardsListCommand ??
                     (getRewardsListCommand = new Command(async () => await ExecuteRewardsListCommand(), () => { return !IsBusy; }));
             }
         }
         
         private async Task ExecuteRewardsListCommand()
         {
             if (IsBusy)
                 return;

             if (ForceSync)
              //Settings.LastSync = DateTime.Now.AddDays(-30);

             IsBusy = true;
             GetRewardsListCommand.ChangeCanExecute();
             var showAlert = false;
             try
             {
                 if (Stores != null)
                    Stores.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = "6";
                trn.Note = "Rewards";
                trn.AgentCode = selectedSection.SupplierId;
                trn.Quantity = selectedSection.Count;
                trn.Product = selectedSection.Description;
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
                string paramlocal = string.Format(HostDomain  + "/Mobile/Transaction/?{0}", myContent);
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
                 GetRewardsListCommand.ChangeCanExecute();
             }

             if (showAlert)
                 await page.DisplayAlert("Oh Oooh :(", "Unable to gather rewards.", "OK");


         }
        #endregion

        #region LoyalRedeemRewards
        public void RedeemSelectedRewards(MenuItem mm)
        {
            redeemSection = mm;
            OnPropertyChanged("SelectedLoyalty");
            if (redeemSection == null)
                return;

            if (ItemSelected == null)
            {
                GetRewardsListCommand.Execute(null);
                RedeemSection = null;
            }
            else
            {
                ItemSelected.Invoke(selectedSection);
            }
        }

        private Command redeemRewardsCommand;

        public Command RedeemRewardsCommand
        {
            get
            {
                return redeemRewardsCommand ??
                    (redeemRewardsCommand = new Command(async () => await ExecuteRedeemRewardsCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteRedeemRewardsCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetRewardsListCommand.ChangeCanExecute();
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
                trn.ProcessingCode = "320000";
                trn.Narrative = "Loyalty";
                trn.Note = "Redeem Points";
                trn.AgentCode = SupplierId;
                trn.Quantity = Quantity;
                trn.ServiceProvider = "Redeem Points";
                trn.TransactionRef = "Mobile";
                trn.ServiceId = ServiceId;
                if (string.IsNullOrEmpty(Balance)) Balance = "0";
                trn.Amount = decimal.Parse(Balance);
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
                    var servics = new List<MenuItem>();
                    var px = new MenuItem();
                    if (response.Narrative != null)
                    {
                        servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        px = servics.FirstOrDefault();
                    }
                    //Stores.ReplaceRange(servics);
                    if (response.ResponseCode == "00000")
                    {
                        await page.DisplayAlert("Success", "Your " + response.Narrative +  " points have been successfully redeemed. " , "OK");
                    }
                    else if(response.ResponseCode == "11333")
                    {
                        //await page.Navigation.PushAsync(new AccountEntry(response));
                        switch (px.TransactionType)
                        {
                            case 9: // Webview  
                                await page.Navigation.PushAsync(new WebviewPage(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                                break;
                            case 3:// "Bill Payment":
                                await page.Navigation.PushAsync(new PayBill(px));
                                break;
                            case 2: // recharge        
                                await page.Navigation.PushAsync(new Recharge(px));
                                break;
                            //case 17: // Cashout         
                            //   await page.Navigation.PushAsync(new Recharge(px));
                            //    break;
                        }
                    }
                    else if (response.ResponseCode == "WEB")
                    {
                        await page.Navigation.PushAsync(new WebviewPage(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                    }
                    else
                    {
                        await page.DisplayAlert("Oh Oooh :(", "Unable to redeem points.", "OK");
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
                GetRewardsListCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to redeem points. Check your internet connectivity", "OK");
        }

        private async void RewardAlerts()
        {
            if (redeemSection.Amount == null) redeemSection.Amount = "0";
            int myPoints = Convert.ToInt32(redeemSection.Amount);
            if (redeemSection.TransactionType > redeemSection.Count)
            {
                if (redeemSection.RequiresAmount == true)
                {
                    Balance = redeemSection.TransactionType.ToString();
                    PTitle = redeemSection.Title;
                    await page.Navigation.PushAsync(new AmountPopup(redeemSection));
                    
                    MessagingCenter.Subscribe<string, MenuItem>("PaymentRequest", "GetAmountMsg", async (sender, arg) =>
                    {
                      
                        if (arg.Note != "Fail")
                        {
                            try
                            {
                                if (Convert.ToInt32(Balance) >= Convert.ToInt32(arg))
                                {
                                    var answer = await page.DisplayAlert("Confirm", "Are you sure you want to redeem the " + PTitle + " Reward worth " + arg + " points?", "Yes", "No");
                                    if (answer == true)
                                    {
                                        Quantity = Convert.ToInt32(arg);
                                        Balance = arg.Amount;
                                        RedeemRewardsCommand.Execute(null);
                                    }
                                }
                                else
                                {
                                    await page.DisplayAlert("Oh Oooh :(", "Your points are not enough for this this reward, you only have " + PTitle + "points ", "OK");
                                }
                            }catch
                            {
                                await page.DisplayAlert("Oh Oooh :(", "Please enter a valid amount", "OK");
                            }
                        }
                        else
                        {
                            await page.DisplayAlert("Oh Oooh :(", "Please enter a valid amount", "OK");
                        }
                        MessagingCenter.Unsubscribe<string, string>("PaymentRequest", "GetAmountMsg");
                    });
                }
                else
                {

                    var answer = await page.DisplayAlert("Confirm", "Are you sure you want to redeem the " + redeemSection.Title + " Reward worth " + redeemSection.Count + " points?", "Yes", "No");
                    if (answer == true)
                    {
                        RedeemRewardsCommand.Execute(null);
                    }
                }
            }
            else
            {
                await page.DisplayAlert("Oh Oooh :(", "Your points are not enough for this this reward, you only have " + redeemSection.TransactionType + "points ", "OK");
            }
            IsBusy = false;
        }
        #endregion

        #region Object

        string balance = string.Empty;
        public string Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }

        string supplierId = string.Empty;
        public string SupplierId
        {
            get { return supplierId; }
            set { SetProperty(ref supplierId, value); }
        }

        string section = string.Empty;
        public string Section
        {
            get { return section; }
            set { SetProperty(ref section, value); }
        }

        string ptitle = string.Empty;
        public string PTitle
        {
            get { return ptitle; }
            set { SetProperty(ref ptitle, value); }
        }

        long serviceId = 0;
        public long ServiceId
        {
            get { return serviceId; }
            set { SetProperty(ref serviceId, value); }
        }

        int quantity =0;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }

        #endregion
    }

}

