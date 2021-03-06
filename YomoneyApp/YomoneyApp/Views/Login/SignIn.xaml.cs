using YomoneyApp.Views.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;
using Newtonsoft.Json;
using YomoneyApp.Models;
using YomoneyApp.Utils;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.ViewModels.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignIn : ContentPage
    {
        AccountViewModel viewModel;
        CountryPickerViewModel countryPickerViewModel;

        string HostDomain = "https://www.yomoneyservice.com";
        string webviewLink = "/Mobile/Forms?SupplierId=5-0001-0001052&serviceId=1&ActionId=3&FormNumber=9&Customer=263784607691&CallType=FirstTime";
        string title = "Agent Sign Up";

        public SignIn()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            countryPickerViewModel = new CountryPickerViewModel(this);

        }

        #region Properties

        //public CountryModel SelectedCountry
        //{
        //    get => _selectedCountry;
        //    set => SetProperty(ref _selectedCountry, value);
        //}

        #endregion Properties

        #region Tap Gestures
        async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            //Navigation.PopAsync();
            await Navigation.PushAsync(new PhoneNumberVerificationPage());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //Navigation.PopAsync();
            await Navigation.PushAsync(new NewAccount());

            //await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + webviewLink, title, false, null, false));
        }
        #endregion

        #region Clicking the Sign In Button
        private void ButtonSignIn_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }
        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //viewModel.SelectedCountry = countryPickerViewModel.SelectedCountry;

            viewModel.ExecuteGetCurrentGeolocationCommand();
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {

        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                if (checkbox.IsChecked == true)
                {
                    checkbox.IsChecked = true;
                    viewModel.IsRememberMe = true;
                }
                else
                {
                    checkbox.IsChecked = false;
                    viewModel.IsRememberMe = false;
                }
            }
        }
    }
}