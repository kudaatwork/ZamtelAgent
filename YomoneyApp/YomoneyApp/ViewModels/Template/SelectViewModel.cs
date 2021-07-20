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
using YomoneyApp.Views.Services;

namespace YomoneyApp
{
    public class SelectViewModel : ViewModelBase
    {
        string HostDomain = "http://192.168.100.150:5000";
        string ProcessingCode = "350000";
        readonly IDataStore dataStore;
        public bool ForceSync { get; set; }
        public ObservableRangeCollection<MenuItem> Actions { get; set; }
        public ObservableRangeCollection<Grouping<string, Store>> ActionsGrouped { get; set; }
        //public bool ForceSync { get; set; }
        public SelectViewModel(Page page,MenuItem selected) : base(page)
        {
            Title = selected.Title;
            dataStore = DependencyService.Get<IDataStore>();
            Actions = new ObservableRangeCollection<MenuItem>();
            ActionsGrouped = new ObservableRangeCollection<Grouping<string, Store>>();
            //GetSeledtedValues(selected);
        }
        public Action<MenuItem> ItemSelected { get; set; }

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
                    switch(selectedAction.Section)
                    {
                        case "Loyalty":
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;
                        case "Service":
                            page.Navigation.PushAsync(new ServiceProviders(selectedAction));
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
                }
                else
                {
                    ItemSelected.Invoke(selectedAction);
                }
            }
        }

        public void GetSeledtedValues(MenuItem mm)
        {
           
                selectedAction = mm;
                OnPropertyChanged("SelectedAction");
                if (selectedAction == null)
                    return;

                if (ItemSelected == null)
                {
               
                 GetActionCommand.Execute(null);
                    SelectedAction = null;  
                }
                else
                {
                   ItemSelected.Invoke(selectedAction);
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
                        await ExecuteGetActionCommand(selectedAction);
                    }));
            }
        }

        private Command getActionCommand;

        public Command GetActionCommand
        {
            get
            {
                return getActionCommand ??
                    (getActionCommand = new Command(async () => await ExecuteGetActionCommand(selectedAction), () => { return !IsBusy; }));
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
            try
            {
          
                Actions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                AccessSettings acnt = new Services.AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                trn.CustomerAccount = uname + ":" + pass;
                //trn.CustomerAccount = "263774090142:22398";
                trn.MTI = "0300";
                trn.ProcessingCode = "420000";
                trn.Narrative = itm.TransactionType.ToString();
                trn.ServiceId = itm.ServiceId;
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

                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    Actions.ReplaceRange(servics);
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
                GetActionCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }

    }

}

