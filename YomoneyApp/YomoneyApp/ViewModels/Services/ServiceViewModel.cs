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
using System.Net;
using System.IO;
using Xamarin.Essentials;
using System.Xml;
using FluentValidation.Validators;
using System.Runtime.InteropServices;
using YomoneyApp.Models;
using YomoneyApp.Models.Work;

namespace YomoneyApp
{ 
    public class ServiceViewModel : ViewModelBase
    {  
        string HostDomain = "https://www.yomoneyservice.com";
        //string ProcessingCode = "350000";
        public ObservableRangeCollection<MenuItem> ServiceProviders { get; set; }

        public ObservableRangeCollection<MenuItem> ServiceOptions { get; set; }

        public ObservableRangeCollection<MenuItem> Currencies { get; set; }

        public bool ForceSync { get; set; }

        public ServiceViewModel(Page page, MenuItem selected) : base(page)
        {
            Title = selected.Title;
            MediaSource = selected.WebLink;
            Description = selected.Description;
            ServiceOptions = new ObservableRangeCollection<MenuItem>();
            ServiceProviders = new ObservableRangeCollection<MenuItem>();
            Currencies = new ObservableRangeCollection<MenuItem>();
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

                if (ItemSelected == null)
                {
                    page.Navigation.PushAsync(new ServiceProviders(selectedCategory));
                    selectedCategory = null;
                    SelectedCategory = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedCategory);
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
                    page.Navigation.PushAsync(new ServiceActions(selectedService));
                    selectedService = null;
                    SelectedService = null;
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
                            MenuItem resp = new MenuItem();
                            resp.Section = JsonConvert.SerializeObject(selectedAction);
                            if (selectedAction.Amount == null) selectedAction.Amount = "0";
                            resp.Amount = selectedAction.Amount;
                            page.Navigation.PushAsync(new AccountEntry(resp));
                            break;
                        case "Payment":
                            page.Navigation.PushAsync(new PaymentPage(selectedAction));
                            break;

                    }

                    //page.Navigation.PushAsync(new SelectPage(selectedAction));

                    selectedAction = null;
                    SelectedAction = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedAction);
                }
            }
        }

        MenuItem selectedProduct;
        public MenuItem SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
                if (selectedProduct == null)
                    return;

                if (ItemSelected == null)
                {

                    if (selectedProduct.HasProducts && selectedProduct.TransactionType == 22)
                    {
                        page.Navigation.PushAsync(new Entertainment(selectedProduct));
                    }
                    else
                    {
                        page.Navigation.PushAsync(new ServiceFiles(selectedProduct));
                    }

                    selectedProduct = null;
                    SelectedProduct = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedProduct);
                }
            }
        }

        MenuItem openFile;
        public MenuItem OpenFile
        {
            get { return openFile; }
            set
            {
                openFile = value;
                OnPropertyChanged("ServiceFile");
                if (openFile == null)
                    return;

                if (ItemSelected == null)
                {
                    switch (openFile.Section)
                    {
                        case "IMAGE":
                            page.Navigation.PushAsync(new LoyaltyActions(openFile));
                            break;
                        case "DOCUMENT":
                            page.Navigation.PushAsync(new ServiceActions(openFile));
                            break;
                        case "AUDIO":
                            MediaSource = openFile.WebLink;
                            openFile.IsShare = true;
                            IsVisible = true;
                            break;
                        case "VIDEO":
                           // PlayFile(openFile);
                            page.Navigation.PushAsync(new VideoAudioPlayer(openFile));
                            
                            break;

                    }

                    OpenFile = null;
                    openFile = null;
                }
                else
                {
                    ItemSelected.Invoke(openFile);
                }
            }
        }

        #region model
        string id;
        public string Id
        {
            get { return id; }
            set
            {
                SetProperty(ref id, value);
            }
        }

        string receiverName;
        public string ReceiverName
        {
            get { return receiverName; }
            set
            {
                SetProperty(ref receiverName, value);
            }
        }

        string currency;
        public string Currency
        {
            get { return currency; }
            set
            {
                SetProperty(ref currency, value);
            }
        }

        string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                SetProperty(ref amount, value);
            }
        }

        string receiverSurname;
        public string ReceiverSurname
        {
            get { return receiverSurname; }
            set
            {
                SetProperty(ref receiverSurname, value);
            }
        }

        string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                SetProperty(ref phoneNumber, value);
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
        string mediaSource;
        public string MediaSource
        {
            get { return mediaSource; }
            set
            {
                SetProperty(ref mediaSource, value);
            }
        }

        bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                SetProperty(ref isVisible, value);
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
        string description;
        public string Description
        {
            get { return description; }
            set
            {
                SetProperty(ref description, value);
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
       
        #region load other Services
        public void GetOtherServices(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {

                GetVariationsCommand.Execute(mm);
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
                string pass =  acnt.Password;
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

        #region load Services Categories
        public void GetServicesCategories(MenuItem mm)
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
                                gtm.Image = HostDomain + "/Content/Spani/Images/myServices.jpg";
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
                trn.TransactionType =itm.TransactionType;
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
                Body += "&TransactionType=" +trn.TransactionType;

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
                    foreach (var gtm in provs )
                    {
                        var i = gtm.FirstOrDefault();
                       // if (i.Note != null)
                       // {
                            MenuItem mn = new MenuItem();
                            
                            mn.Title = i.Note;
                            if (i.Image == null)
                            {
                                if (itm.Image  != null)
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

        #region Voucher Purchase
        Command getVoucherCommand;
        public Command GetVoucherCommand
        {
            get
            {
                return getVoucherCommand ??
                    (getVoucherCommand = new Command(async () => await ExecuteGetVoucherCommand(renderService), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetVoucherCommand(MenuItem mnu)
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Amount) || string.IsNullOrWhiteSpace(ReceiverName) || string.IsNullOrWhiteSpace(ReceiverSurname) ||
                string.IsNullOrWhiteSpace(Id))
            {
                await page.DisplayAlert("Enter All Fields", "Please enter all fields", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                TransactionRequest trn = new RetailKing.Models.TransactionRequest();
               
                    MenuItem mn = new YomoneyApp.MenuItem();
                    char[] delimiter = Currency.ToCharArray();
                    var amt = Amount;

                    mn.Amount = Amount;
                    mn.Currency = Currency;
                    mn.Title = mnu.Title;

                    
                   await page.Navigation.PushModalAsync(new PaymentPage(mn));

                    IsBusy = false;
                    MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (sender, arg) =>
                    {
                        if (arg == "Payment Success")
                       {
                            AccessSettings acnt = new Services.AccessSettings();
                            string pass = acnt.Password;
                            string uname = acnt.UserName;

                            #region Voucher Data
                            CustomerService cs = new CustomerService();
                            
                            cs.CustomerMobileNumber = acnt.UserName;
                            cs.ReceiverMobile = PhoneNumber;
                            cs.ReceiversName = receiverName;
                            cs.ReceiversSurname = receiverSurname;
                            cs.Balance = decimal.Parse(Amount);
                            cs.ReceiversIdentification = id;
                            cs.Currency = Currency;
                            #endregion

                            trn.CustomerAccount = uname + ":" + pass;
                            trn.MTI = "0200";
                            trn.ProcessingCode = "320000";
                            //trn.Narrative = JsonConvert.SerializeObject(jp);
                            trn.Note = "Supplier";
                            trn.Amount = decimal.Parse(Amount);
                            trn.AgentCode = mnu.SupplierId;
                            trn.ServiceId = mnu.ServiceId;
                            trn.ServiceProvider = mnu.SupplierId;
                            trn.TransactionType = 2;
                            trn.Currency = currency;
                            trn.ActionId = mnu.ActionId;
                            trn.CustomerData = JsonConvert.SerializeObject(cs);
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
                            Body += "&TransactionType=" + trn.TransactionType;
                            Body += "&Currency=" + trn.Currency;
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
                                    //Message = "Job Awarded Successfully";
                                    await page.DisplayAlert("Success", "You Have successfully Purchased an eVoucher for " + ReceiverName, "OK");
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
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            await page.DisplayAlert("Payment Failed", "Unable to Award Job, the 5% commitment fee was not paid.", "OK");
                        }
                       // MessagingCenter.Unsubscribe<string, string>("PaymentRequest", "NotifyMsg");
                    });
                
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", "Unable to Award Job, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                //awardBidCommand?.ChangeCanExecute();
            }

        }
        #endregion

        #region load services
        public void GetSeledtedProvider(MenuItem mm)
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
                    var grp = servics.GroupBy(u => u.ServiceId).ToList();
                    foreach (var serv in servics)
                    {
                        if (serv.Image != null)
                        {
                            char[] delimiters = new char[] { '~' };
                            string[] supid = serv.Image.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                            serv.Image = HostDomain + supid[0];
                        }
                        else if(img != null)
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
            if(itm.Section == "Loyalty")
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
                    if (itm.Id != null)
                    {
                        trn.ActionId = long.Parse(itm.Id);
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
                    Body += "&ActionId=" + trn.ActionId;
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
                await page.DisplayAlert("Error", itm.Title + " failed to load. Check your internet connection", "OK");

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

        public async Task ExecuteRenderActionCommand(MenuItem itm)
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
                if (!string.IsNullOrEmpty(itm.WebLink))
                {
                    if (itm.WebLink.Contains("/Mobile/") || itm.WebLink.Contains("/Mobile//"))
                    {
                        itm.WebLink = itm.WebLink.Trim() + "&Customer=" + uname;
                    }
                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(itm.WebLink, itm.Title, false, itm.ThemeColor));
                    return;
                }
                trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                trn.Narrative = "Service";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Product = itm.Id;
                if (itm.Id != null)
                {
                    trn.ActionId = long.Parse(itm.Id);
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
                Body += "&ActionId=" + trn.ActionId;
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
                    
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        px = servics.FirstOrDefault();
                        switch (px.TransactionType)
                        {
                            case 10:
                            case 9: // webview  
                                if (px.Note == "External")
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false,px.ThemeColor ));
                                }
                                else
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                                }
                                break;
                            case 2:
                            case 3:// "Payment":
                                if (px.HasProducts)
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false, px.ThemeColor));
                                    //await page.Navigation.PushAsync(new ServiceActionProducts(px));
                                }
                                else
                                {
                                    if (px.Section == "SHOPPING VOUCHERS")
                                    {
                                        await page.Navigation.PushAsync(new BuyVoucher(px));
                                    }
                                    else
                                    {
                                        if (px.Amount == "0" || px.Amount == null)
                                        {
                                            await page.Navigation.PushAsync(new AmountPopup(px));
                                        }
                                        else
                                        {
                                            await page.Navigation.PushAsync(new PaymentPage(px));
                                        }
                                    }
                                }
                                break;
                            case 5:
                                if (px.HasProducts)
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false, px.ThemeColor));
                                    //await page.Navigation.PushAsync(new ServiceActionProducts(px));
                                }
                                else
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false, px.ThemeColor));
                                }
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
                            case 17: // Signature 
                                await page.Navigation.PushAsync(new AccountEntry(px));
                                break;
                            case 22: // Entertainment 
                                await page.Navigation.PushAsync(new ServiceActionProducts(px));
                                break;
                            default:
                                if (px.Note == "External")
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(px.Section, px.Title, false, px.ThemeColor));
                                }
                                else
                                {
                                    await page.Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + px.Section, px.Title, false, px.ThemeColor));
                                }
                                break;
                               
                        }

                    }
                    else
                    {
                        if (response.ResponseCode == "11102")
                        {
                            await page.DisplayAlert("Service Maintenance ", "Service is under maintenance please try again later", "OK");
                        }
                        else
                        {
                            await page.DisplayAlert("Transaction Error ", "Please try again letter ", "OK");
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
                RenderActionCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Service Error", "Unable to load " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region Get Action Products
        public void GetActionProducts(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("selectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                ActionProductsCommand.Execute(null);
               
                //  SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            }
        }

        private Command actionProductsCommand;

        public Command ActionProductsCommand
        {
            get
            {
                return actionProductsCommand ??
                    (actionProductsCommand = new Command(async () => await ExecuteActionProductsCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteActionProductsCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            ActionProductsCommand.ChangeCanExecute();
            var showAlert = false;
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
                trn.ProcessingCode = "430000";
                trn.Narrative = "Service";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Product = itm.Id;
                trn.ActionId = itm.Count;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&ActionId=" + trn.ActionId;
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
                    
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        px = servics.FirstOrDefault();
                        ServiceOptions.ReplaceRange(servics);
                        
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

        #region Get MediaFiles
        public void GetMediaFile(MenuItem mm)
        {

            selectedProduct = mm;
            OnPropertyChanged("ServiceFile");
            if (selectedProduct == null)
                return;

            if (ItemSelected == null)
            {

                MediaFileCommand.Execute(null);
              //  SelectedService = null;
            }
            else
            {
                ItemSelected.Invoke(selectedProduct);
            }
        }

        private Command mediaFileCommand;

        public Command MediaFileCommand
        {
            get
            {
                return mediaFileCommand ??
                    (mediaFileCommand = new Command(async () => await ExecuteMediaFileCommand(selectedProduct), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteMediaFileCommand(MenuItem itm)
        {
            if (IsBusy)
                return;
            if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays(-30);

                IsBusy = true;
            MediaFileCommand.ChangeCanExecute();
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
                trn.ProcessingCode = "480000";
                trn.Narrative = "Service";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Product = itm.Id;
                trn.ActionId = itm.Count;
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&ActionId=" + trn.ActionId;
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
                    
                    if (response.ResponseCode == "00000")
                    {
                        var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                        px = servics.FirstOrDefault();
                        foreach (var fl in servics)
                        {
                            if (fl.WebLink != null)
                            {
                                fl.IsShare = false;
                            }
                            else
                            {
                                servics.Remove(fl);
                            }
                        }
                        ServiceOptions.ReplaceRange(servics);
                       // await page.Navigation.PushAsync(new OTPPage(px)); x.ThemeColor));
                                
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

        #region download file   
        private Command saveFileCommand;

        public Command SaveFileCommand
        {
            get
            {
                return saveFileCommand ??
                    (saveFileCommand = new Command(async () => await ExecuteSaveFileCommand(openFile), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteSaveFileCommand(MenuItem itm)
        {
       
            var _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(360000) };

            try
            {
                using (var httpResponse = await _httpClient.GetAsync(itm.WebLink))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var YourFilePath = FileSystem.AppDataDirectory;
                        var ReturnedBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                        if (itm.Section == "AUDIO")
                        {
                            File.WriteAllBytes(YourFilePath, ReturnedBytes);
                        }
                        else if (itm.Section == "VIDEO")
                        {
                            File.WriteAllBytes(YourFilePath, ReturnedBytes);
                        }
                        else if (itm.Section == "IMAGES")
                        {
                            File.WriteAllBytes(YourFilePath, ReturnedBytes);
                        }
                        else
                        {
                            File.WriteAllBytes(YourFilePath, ReturnedBytes);
                        }
                    }
                    else
                    {
                        //Url is Invalid
                       // return null;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
               // return null;
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

        }
        #endregion

        #region Objects
        string purpose = string.Empty;
        public string Purpose
        {
            get { return purpose; }
            set { SetProperty(ref purpose, value); }
        }

        string supplierCode = string.Empty;
        public string SupplierCode
        {
            get { return supplierCode; }
            set { SetProperty(ref supplierCode, value); }
        }

        string serviceId = string.Empty;
        public string ServiceId
        {
            get { return serviceId; }
            set { SetProperty(ref serviceId, value); }
        }

        string actionId = string.Empty;
        public string ActionId
        {
            get { return actionId; }
            set { SetProperty(ref actionId, value); }
        }

        string formId = string.Empty;
        public string FormId
        {
            get { return formId; }
            set { SetProperty(ref formId, value); }
        }

        string fieldId = string.Empty;
        public string FieldId
        {
            get { return fieldId; }
            set { SetProperty(ref fieldId, value); }
        }

        string customerPhoneNumber = string.Empty;
        public string CustomerPhoneNumber
        {
            get { return customerPhoneNumber; }
            set { SetProperty(ref customerPhoneNumber, value); }
        }
        #endregion

    }

}

