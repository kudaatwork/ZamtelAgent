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
using YomoneyApp.Views.Promotions;

namespace YomoneyApp
{
    public class PromotionsViewModel : ViewModelBase
    {
        
        string HostDomain = "http://192.168.100.172:5000";
        //string ProcessingCode = "350000";
        public ObservableRangeCollection<MenuItem> ServiceProviders { get; set; }
        public ObservableRangeCollection<MenuItem> ServiceOptions { get; set; }
        public ObservableRangeCollection<MenuItem> ServiceList { get; set; }
        public MenuItem PromoDetail { get; set; }
        public bool ForceSync { get; set; }
        public PromotionsViewModel(Page page, MenuItem selected) : base(page)
        {
            Title = selected.Title;
            ServiceList = new ObservableRangeCollection<MenuItem>();
            ServiceOptions = new ObservableRangeCollection<MenuItem>();
            ServiceProviders = new ObservableRangeCollection<MenuItem>();
            PromoDetail = new MenuItem();
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
                    if (selectedCategory.Title == "REWARD SCHEMES" || selectedCategory.Title == "LUCKY DRAW")
                    {
                        page.Navigation.PushAsync(new ServiceProviders(selectedCategory));
                        selectedCategory = null;
                        SelectedCategory = null;

                    }
                    else
                    {
                        page.Navigation.PushAsync(new ProviderPromotions(selectedCategory));
                        selectedCategory = null;
                        SelectedCategory = null;
                    }
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
                    if (selectedService.Title == "REWARD SCHEMES")
                    {
                        page.Navigation.PushAsync(new ServiceActions(selectedService));
                        selectedService = null;
                        SelectedService = null;
                    }
                    else
                    {
                        page.Navigation.PushAsync(new SharePage(selectedService));
                        selectedService = null;
                        SelectedService = null;
                    }
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
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;
                        case "Payment":
                            page.Navigation.PushAsync(new LoyaltyActions(selectedAction));
                            break;

                    }
                    // page.Navigation.PushAsync(new SelectPage(selectedAction));
                    SelectedAction = null;
                    selectedAction = null;
                }
                else
                {
                    ItemSelected.Invoke(selectedAction);
                }
            }
        }

        #region model
        
        bool showSearchList =false;
        public bool ShowSearchList
        {
            get { return showSearchList; }
            set
            {
                SetProperty(ref showSearchList, value);
            }
        }
        bool hideSearchList= true;
        public bool HideSearchList
        {
            get { return hideSearchList; }
            set
            {
                SetProperty(ref hideSearchList, value);
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

        #region load other Services
        public void GetOtherServices(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {

                GetVariationsCommand.Execute(null);
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

        #region load Promotion Categories
        public void GetPromotionCategories(MenuItem mm)
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
                                gtm.Image =  itm.Image;
                            }
                            else
                            {
                                gtm.Image = HostDomain + "/Content/Spani/Images/Campaign.ong";
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
        
        #region load services
        public void GetSelectedProvider(MenuItem mm)
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

        #region load Promotion
        public void GetCurrentPromotions(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetPromoCommand.Execute(mm);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            };
            
        }

        private Command getPromoCommand;

        public Command GetPromoCommand
        {
            get
            {
                return getPromoCommand ??
                    (getPromoCommand = new Command(async () => await ExecuteGetPromoCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetPromoCommand(MenuItem itm)
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

                ServiceList.Clear();
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
                trn.ServiceProvider = "PROMOTION LIST";
                trn.AgentCode = itm.SupplierId;
                trn.Note = "PROMOTION LIST";
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
                    foreach(var it in servics)
                    {
                        if(it.MediaType.Trim() == "Image")
                        {
                            it.IsAdvert = true;
                            it.IsNotAdvert = false;
                            it.Media = "http://192.168.100.172:5000/Content/notify/notify.mp3";
                        }
                        else
                        {
                            it.IsAdvert = false;
                            it.IsNotAdvert = true;
                            it.Media = it.Image;
                            it.Image = "InsuranceBanner.png";
                        }
                        if(string.IsNullOrEmpty(it.UserImage))
                        {
                            it.UserImage = "http://192.168.100.172:5000/Content/Administration/images/user.png"; ;
                        }
                    }
                    ServiceList.ReplaceRange(servics);
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

            //if (showAlert)
              //  await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion

        #region load Promotion
        public void GetSearchPromotions(MenuItem mm)
        {

            selectedAction = mm;
            OnPropertyChanged("SelectedAction");
            if (selectedAction == null)
                return;

            if (ItemSelected == null)
            {
                GetSearchCommand.Execute(mm);
                SelectedAction = null;
            }
            else
            {
                ItemSelected.Invoke(selectedAction);
            };

        }

        private Command getSearchCommand;

        public Command GetSearchCommand
        {
            get
            {
                return getSearchCommand ??
                    (getSearchCommand = new Command(async () => await ExecuteGetSearchCommand(selectedAction), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetSearchCommand(MenuItem itm)
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

                ServiceList.Clear();
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
                trn.ServiceProvider = "PROMOTION SEARCH";
                trn.AgentCode = itm.SupplierId;
                trn.Note = "PROMOTION SEARCH";
                trn.CustomerData = itm.Note;
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
                    foreach (var it in servics)
                    {
                        if (it.MediaType.Trim() == "Image")
                        {
                            it.IsAdvert = true;
                            it.IsNotAdvert = false;
                            it.Media = "http://192.168.100.172:5000/Content//notify//notify.mp3";
                        }
                        else
                        {
                            it.IsAdvert = false;
                            it.IsNotAdvert = true;
                            it.Media = it.Image;
                            it.Image = "InsuranceBanner.png";
                        }
                        if (string.IsNullOrEmpty(it.UserImage))
                        {
                            it.UserImage = "http://192.168.100.172:5000/Content/Administration/images/user.png"; ;
                        }
                    }
                    ServiceList.ReplaceRange(servics);
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

            //if (showAlert)
            //  await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

        }
        #endregion


        #region SwitchView
        public void SwitchViews()
        {
            ShowSearchList = true;
            hideSearchList = false;
           
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
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather " + itm.Title + ". Check your internet connection", "OK");

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

        private async Task ExecuteRenderActionCommand(MenuItem itm)
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
                trn.MTI = "0200";
                trn.ProcessingCode = "320000";
                trn.Narrative = "Service";
                trn.TransactionType = itm.TransactionType;
                trn.ServiceId = itm.ServiceId;
                trn.ServiceProvider = itm.Section;
                trn.AgentCode = itm.SupplierId;
                trn.Product = itm.Id;
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
                    var px = new MenuItem();
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    var servics = JsonConvert.DeserializeObject<List<MenuItem>>(response.Narrative);
                    px = servics.FirstOrDefault();
                    if (response.ResponseCode == "00000")
                    {
                        switch (px.TransactionType)
                        {
                            case 9: // webview  
                                await page.Navigation.PushAsync(new WebviewPage(HostDomain + px.Section , px.Title, false,px.ThemeColor));
                                break;
                            case 3 :// "Payment":
                                await page.Navigation.PushAsync(new ServicePayment(px));
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
                        }

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
            // await page.Navigation.PopAsync();
        }
        #endregion

    }

}

