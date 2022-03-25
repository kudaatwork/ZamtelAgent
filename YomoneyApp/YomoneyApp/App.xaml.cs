using System;
using System.IO;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;
using YomoneyApp.Views.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YomoneyApp
{    
    public partial class App : Application
    {
        public App(string dbPath)
        {
            InitializeComponent();
            
            AccessSettings ac = new Services.AccessSettings();
            string pass = ac.Password;
            App.MyLogins  = "";
            string s = ac.SaveValue("dbPath", dbPath).Result;
            //MyLogins = new MyLogins();
            Page pg = new Page();
            ChatViewModel cvm = new ChatViewModel(pg);
            ChatServices sigChat = new ChatServices();
           
            if (!string.IsNullOrEmpty(pass))
            {

                //MenuItem menuItem = new MenuItem();

                //menuItem.Id = "1";
                //menuItem.Image = "https://www.yomoneyservice.com/Content/Logos/ZAMTEL/zamtel.png";
                //menuItem.Title = "SIM CARD MANAGEMENT";
                //menuItem.Description = "SIM CARD MANAGEMENT";
                //menuItem.Section = "Service";
                //menuItem.Note = "ZAMTEL";
                //menuItem.ServiceId = 1;
                //menuItem.TransactionType = 12;
                //menuItem.SupplierId = "5-0001-0001052";
                //menuItem.date = "0001-01-01T00:00:00";

                //await page.Navigation.PushAsync(new ServiceActions(menuItem));


                #region Load Services
                MenuItem menuItem = new MenuItem();

                menuItem.Id = "1";
                menuItem.Image = "https://www.yomoneyservice.com/Content/Logos/ZAMTEL/zamtel.png";
                menuItem.Title = "WAFAYA";
                menuItem.Note = "BANKING";
                menuItem.TransactionType = 12;
                menuItem.SupplierId = "5-0001-0001052";
                //menuItem.date = "0001-01-01T00:00:00";

                //await page.Navigation.PushAsync(new ProviderServices(menuItem));
                #endregion


                // cvm.GetSupportCommand.Execute(null);
                MainPage = new NavigationPage(new ProviderServices(menuItem))
                {
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#22b24c")
                };

                //MainPage = new NavigationPage(new HomePage())
                //{
                //    BarTextColor = Color.White,
                //    BarBackgroundColor = Color.FromHex("#22b24c")
                //};

                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                {
                    if (!sigChat.IsConnectedOrConnecting)
                        cvm = new ChatViewModel(pg);
                    return true;
                });
            }
            else
            {               
                MainPage = new NavigationPage(new AccountMain())
                {
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#22b24c")
                };

            }
        }

        public static string MyLogins;

        public static string AuthToken;
                        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            Page pg = new Page();
            ChatViewModel cvm = new ChatViewModel(pg);
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            Page pg = new Page();
            ChatViewModel cvm = new ChatViewModel(pg);
            // Handle when your app resumes
        }
    }
}
