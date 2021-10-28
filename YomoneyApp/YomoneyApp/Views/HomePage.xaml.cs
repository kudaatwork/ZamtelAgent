using YomoneyApp.Views;
using YomoneyApp.Views.Chat;
using YomoneyApp.Views.Profile;
using YomoneyApp.Views.Profile.Loyalty;
using YomoneyApp.Views.Promotions;
using YomoneyApp.Views.QRScan;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.Spani;
using YomoneyApp.Views.Spend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using YomoneyApp.Views.TransactionHistory;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Fileuploads;
using YomoneyApp.Views.Login;

namespace YomoneyApp
{
    public partial class HomePage : ContentPage
    {
        HomeViewModel viewModel;
        ChatViewModel viewM;
        public HomePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new HomeViewModel(this);
            viewModel.Points = "0.00";
            viewModel.Credit = "--:--";
            viewM = new YomoneyApp.ChatViewModel(this); //ViewModelLocator.ChatMainViewModel;

            viewModel.ExecuteGetDashboardItemsCommand();
           
            if (Device.Idiom == TargetIdiom.Tablet)
            {
               // HeroImage.Source = ImageSource.FromFile("herotablet.jpg");
            }

            MenuItem selectedAction = new MenuItem 
            { 
                Title = "Transaction History", 
                Description = "View your spending history", 
                Image = "Paymenu.png", 
                Section = "Yomoney", 
                ServiceId = 7, 
                SupplierId = "All", 
                TransactionType = 3 
            };
                        
            var Profile = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    AccessSettings acnt = new AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;
                    //Navigation.PushAsync(new WebviewPage("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));

                    Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false, null));
                }),

                Text = "My Profile",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            var PImage = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();
                    px.Title = "Profile Image";
                    Navigation.PushAsync(new FileUploadPage(px));
                }),

                Text = "Profile Picture",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            var Dashboard = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();
                    px.Title = "DashBoard";
                    Navigation.PushAsync(new WaletServices(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));
                }),

                Text = "DashBoard",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            var trsansactionHistory = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();

                    px.Title = "Transaction History";
                    AccessSettings acnt = new AccessSettings();
                    string uname = acnt.UserName;

                    var existingPages = Navigation.NavigationStack.ToList();

                    int cnt = 2;

                    foreach (var page in existingPages)
                    {
                        if (cnt < existingPages.Count)
                        {
                            Navigation.RemovePage(page);
                        }
                        cnt++;
                    }

                     Navigation.PushAsync(new Transactions());
                }),

                Text = "Transaction History",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            var customerSupport = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();

                    px.Title = "Customer Support";
                    AccessSettings acnt = new AccessSettings();
                    string uname = acnt.UserName;

                    var existingPages = Navigation.NavigationStack.ToList();

                    int cnt = 2;

                    foreach (var page in existingPages)
                    {
                        if (cnt < existingPages.Count)
                        {
                            Navigation.RemovePage(page);
                        }
                        cnt++;
                    }

                    //Navigation.PushAsync(new CustomerSupport())

                    DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");

                }),

                Text = "Customer Support",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            var signOut = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();

                    px.Title = "Sign Out";
                    AccessSettings ac = new Services.AccessSettings();
                    ac.DeleteCredentials();
                    Navigation.PushAsync(new AccountMain());
                }),

                Text = "Sign Out",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };           

            ToolbarItems.Add(Dashboard);
            ToolbarItems.Add(Profile);
            ToolbarItems.Add(PImage);
            ToolbarItems.Add(trsansactionHistory);
            //ToolbarItems.Add(customerSupport);
            ToolbarItems.Add(signOut);

            //topCorousel.GestureRecognizers.Add(gesture);

            ButtonPay.Clicked += async (sender, e) =>
            {
               await Navigation.PushAsync(new QRScanPage());
            };

            ButtonContacts.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new ChatTabs());
            };

            //ButtonSpani.Clicked += (sender, e) =>
            //{
            //    // await Navigation.PushAsync(new SpaniHome());
            //    viewModel.CheckUserCommand.Execute(null);
            //};

            //ButtonInsure.Clicked += async (sender, e) =>
            //{
            //    MenuItem mnu = new MenuItem();
            //    mnu.Title = "PROMOTIONS";
            //    mnu.TransactionType = 23;
            //    mnu.Section = "PROMOTIONS";
            //    mnu.SupplierId = "All";
               
            //    await Navigation.PushAsync(new PromotionCategories(mnu));
            //};

            //ButtonBanking.Clicked += async (sender, e) =>
            //{
            //    MenuItem mnu = new MenuItem();
            //    //mnu.Title = "Investment";
            //    mnu.TransactionType = 12;
            //    mnu.Section = "Supplier Services";
            //    mnu.ServiceId = 12;
            //    mnu.SupplierId = "All";
            //    //await Navigation.PushAsync(new ServiceCategories(mnu));
            //    mnu.Title = "Services";
            //    await Navigation.PushAsync(new ServiceVariations(mnu));
                
            //};

            //ButtonEnjoy.Clicked += async (sender, e) =>
            //{
            //    if (Navigation.NavigationStack.Count == 0 ||
            //        Navigation.NavigationStack.Last().GetType() != typeof(SpendMenu))
            //    {
            //        await Navigation.PushAsync(new SpendMenu());
            //    }
            //};

            //ButtonPoints.Clicked += async (sender, e) =>
            //{
            //    MenuItem mnu = new MenuItem();
            //    mnu.Title = "Yomoney Points";
            //    mnu.TransactionType = 6;
            //    mnu.Section = "Supplier";
            //    mnu.SupplierId = "5-0001-0000000";
            //    mnu.Description = "YoLifestyle";
               
            //    await Navigation.PushAsync(new LoyaltyRewards(mnu));
            //};

            //ButtonCredits.Clicked += async (sender, e) =>
            //{
            //    await Navigation.PushAsync(new TopupPage());
            //};

        }
        private async void Section_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);
                switch(mn.Title.ToUpper())
                {
                    case "AIRTIME":
                    case "RECHARGE":
                       await Navigation.PushAsync(new Recharge(mn));
                        break;
                    case "BILL PAYMENTS":
                        await Navigation.PushAsync(new PayBill(mn));
                        break;
                    case "PROMOTIONS":              
                        mn.Title = "PROMOTIONS";
                        mn.TransactionType = 23;
                        mn.Section = "PROMOTIONS";
                        mn.SupplierId = "All";
                        await Navigation.PushAsync(new PromotionHome(mn));
                        break;
                    case "JOBS":
                        viewModel.CheckUserCommand.Execute(null);
                        break;
                    case "SERVICES":
                        MenuItem mnu = new MenuItem();
                        //mnu.Title = "Investment";
                        mn.TransactionType = 12;
                        mn.Section = "Supplier Services";
                        mn.ServiceId = 12;
                        mn.SupplierId = "All";
                        //await Navigation.PushAsync(new ServiceCategories(mnu));
                        mn.Title = "Services";
                        await Navigation.PushAsync(new ServiceVariations(mn));
                        break;
                    case "TASKS":
                        AccessSettings acnt = new AccessSettings();
                        string uname = acnt.UserName;
                        string link = "https://www.yomoneyservice.com/Mobile/Projects?Id=" + uname;
                        string title = "My Tasks";

                        //string link = "http://102.130.120.163:8090/Login/PosLoginM?username=Salesaqu&password=admin";

                        //string link = "https://www.yomoneyservice.com/Mobile/invokeCsAction?Id=" + "263774090142&message=Kuda";

                        await Navigation.PushAsync(new WebviewHyubridConfirm(link, title, true, "#df782d"));

                        //await Navigation.PushModalAsync(new WebviewPage(link, "My Tasks", true, "#df782d"));
                        MessagingCenter.Subscribe<string, string>("Route", "OpenMap", async (sender, arg) =>
                        {
                            if (!string.IsNullOrEmpty(arg))
                            {
                                // call your map
                            }
                        });
                        break;
                 default:
                        break;
                }
                //ServiceViewModel svm = new ServiceViewModel(this, mn);
                //svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Advert_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                ServiceViewModel svm = new ServiceViewModel(this, mn);
                svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async void MyWallet(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(WaletServices))
            {
                await Navigation.PushAsync(new WaletServices(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.ExecuteGetMenusCommand();//.Execute(null);

           // await viewModel.ExecuteGetAccountCommand();//.Execute(null);
            await viewModel.ExecuteGetStoresCommand();// viewModel.GetAccountCommand.Execute(null);

            try
            {
                AccessSettings ac = new AccessSettings();
                string dbPath = ac.GetSetting("dbPath").Result;
                var db = new YomoneyRepository(dbPath);

                    var yoconts = await db.GetContactsAsync();
                    if(yoconts == null || yoconts.Count > 0)
                    {
                        await viewM.ExecuteGetContactsCommand();
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}

