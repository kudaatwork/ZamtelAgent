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

namespace YomoneyApp.ViewModels.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignIn : ContentPage
    {
        AccountViewModel viewModel;
        CountryPickerViewModel countryPickerViewModel;

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
            await Navigation.PushAsync(new ForgotPasswordOptionPage());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAccount());
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
    }
}