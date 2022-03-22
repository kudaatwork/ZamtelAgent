using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels;
using YomoneyApp.Views.GeoPages;
using YomoneyApp.Views.Services;
using YomoneyApp.Views.TemplatePages;

namespace YomoneyApp.Views.Webview
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebviewHyubridConfirm : ContentPage
    {
        HomeViewModel homeViewModel;
        ChatViewModel viewM;
        MenuItem mnu;

      //  MapPageViewModel mapPage = new MapPageViewModel(null);

       // AccountViewModel accountViewModel = new AccountViewModel(null);

        public WebviewHyubridConfirm(string sourceUrl, string title, bool isModal, string navcolour, bool showNav = true)
        {
            InitializeComponent();
            BindingContext = homeViewModel = new HomeViewModel(this);
            viewM = new YomoneyApp.ChatViewModel(this); //ViewModelLocator.ChatMainViewModel;
           
            if (sourceUrl.Contains("Mobile") || showNav == false)
            {
                NavigationPage.SetHasNavigationBar(this,false);
                //((NavigationPage)Application.Current.MainPage).IsVisible = false;
                //homeViewModel.ShowNav = false;
            }
            else
            {
                NavigationPage.SetHasNavigationBar(this, true);
            }

            homeViewModel.Title = title;

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
                    mnu.ThemeColor = navcolour;
                    var themeColor = Color.FromHex(navcolour);

                    ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = themeColor;
                }
                else
                {
                    mnu.ThemeColor = "#22b24c";
                }

                mnu.IsShare = true;
                mnu.IsEmptyList = false;
                BindingContext = mnu;
                //  url = sourceUrl;
                homeViewModel.Source = sourceUrl;

            //ButtonClose.Clicked += async (sender, e) =>
            //{
            //    await App.Current.MainPage.Navigation.PopModalAsync();
            //    //await Navigation.PopModalAsync();
            //};

                var hybridWebView = new HybridWebView
                {
                    Uri = sourceUrl
                };

            hybridWebView.RegisterAction(data => homeViewModel.DisplayMap(data));

            Padding = new Thickness(0, 0, 0, 0);
            Content = hybridWebView;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //await progress.ProgressTo(0.9, 900, Easing.SpringIn);
        }        

        void webOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
           // progress.IsVisible = false;
            try
            {
                switch (e.Result)
                {
                    case WebNavigationResult.Cancel:

                        Navigation.PopAsync();
                        break;

                    case WebNavigationResult.Failure:

                        var isConnected = Connectivity.NetworkAccess;

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

        private void Webview_Navigating(object sender, WebNavigatingEventArgs e)
        {           
          // progress.IsVisible = true;           
        }
    }
}