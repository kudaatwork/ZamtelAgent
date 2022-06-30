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
            string rememberMe = ac.RememberMe;
            App.MyLogins  = "";
            string s = ac.SaveValue("dbPath", dbPath).Result;
            //MyLogins = new MyLogins();
            Page pg = new Page();
            ChatViewModel cvm = new ChatViewModel(pg);
            ChatServices sigChat = new ChatServices();
           
            if (!string.IsNullOrEmpty(rememberMe))
            {
                if (rememberMe.ToUpper().Trim() == "REMEMBERME")
                {
                    MainPage = new NavigationPage(new HomePage())
                    {
                        BarTextColor = Color.White,
                        BarBackgroundColor = Color.FromHex("#22b24c")
                    };

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

        public static string RememberMe;

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
