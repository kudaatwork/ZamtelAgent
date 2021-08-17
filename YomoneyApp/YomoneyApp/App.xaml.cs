using System;
using System.IO;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;

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
                // cvm.GetSupportCommand.Execute(null);
                MainPage = new NavigationPage(new HomePage())
                {
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#df782d")
                };

                Device.StartTimer(TimeSpan.FromSeconds(30), () => {
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
                    BarBackgroundColor = Color.FromHex("#df782d")
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
