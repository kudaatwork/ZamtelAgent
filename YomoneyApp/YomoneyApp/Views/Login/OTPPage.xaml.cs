using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OTPPage : ContentPage
    {
        AccountViewModel viewModel;

        public OTPPage(string phone)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);

            if (string.IsNullOrEmpty(phone))
            {
                viewModel.PhoneNumber = phone;
            }
            else 
            {
                viewModel.PhoneNumber = phone;
            }
            
            viewModel.GetVerificationAsync();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

            if (!string.IsNullOrEmpty(action))
            {
                try
                {
                    Device.OpenUri(new Uri("https://wa.link/fehh38"));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                }
            }
        }

        private void ButtonSignIn_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            return true;
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var resendCode = await DisplayAlert("Alert!", "Do you really want to resend your verification code?", "Yes", "No");

            if (resendCode)
            {
                viewModel.GetVerificationAsync();
            }
        }
    }
}