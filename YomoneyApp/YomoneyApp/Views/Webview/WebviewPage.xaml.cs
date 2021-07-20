using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Views.Services;

namespace YomoneyApp.Views.Webview
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebviewPage : ContentPage
    {
        
        MenuItem mnu;
        SpendViewModel viewModel;
        string url = "";
        bool Error = false;
        string account = "";
        string uname = "";
        public WebviewPage(string source, string title, bool isModal, string navcolour)
        {
            InitializeComponent();
            mnu = new MenuItem();
            mnu.HasProducts = true;
            mnu.Title = title;
            mnu.IsAdvert = false;
            if (isModal) // used to determin if navigation should show
            {
                mnu.IsAdvert = true;
            }
            if (!string.IsNullOrEmpty(navcolour)) // used to determin if navigation should show
            {
                mnu.ThemeColor  = navcolour;
                var themeColor = Color.FromHex(navcolour);
                ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = themeColor;
            }
            else
            {
                mnu.ThemeColor = "#e2762b";
            }
            mnu.IsShare = true;
            mnu.IsEmptyList = false;
            BindingContext = mnu;
            url = source;
            Webview.Source = source;

            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };
        }
        
        public void ShareClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                foreach (var tkn in viewModel.Stores)
                {
                    viewModel.GetShareToken(tkn);
                }
            }
            catch
            {
            }

        }
        public void ConfirmClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                viewModel.GetTokenCommand.Execute(null);
            }
            catch
            { }

        }
        public void CancelClicked(object sender, EventArgs e)
        {
            try
            {
                App.Current.MainPage.Navigation.PopModalAsync();
                //Navigation.PopModalAsync();
            }
            catch { }
         
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await progress.ProgressTo(0.9, 900, Easing.SpringIn);
            
        }
        void webOnNavigating(object sender, WebNavigatingEventArgs e)
        {
           progress.IsVisible = true;
        }

        void webOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            progress.IsVisible = false;
            try
            {
                    switch (e.Result)
                    {
                        case WebNavigationResult.Cancel:
                        Navigation.PopAsync();
                        break;
                        case WebNavigationResult.Failure:
                        var isConnected =  Connectivity.NetworkAccess;
                        if (isConnected != NetworkAccess.Internet)
                        {
                            Navigation.PushModalAsync(new Offline());
                            Navigation.PopAsync();
                        }
                        break;
                        case WebNavigationResult.Success:
                            mnu.IsEmptyList = false;
                        break;
                        case WebNavigationResult.Timeout:
                        Navigation.PushModalAsync(new Offline());
                        Navigation.PopAsync();
                        break;
                        default:
                        Navigation.PushModalAsync(new Offline());
                        Navigation.PopAsync();
                        break;
                    }

            }
            catch
            {
                Navigation.PushModalAsync(new Offline());
                Navigation.PopAsync();
            }
        }  
    }
}