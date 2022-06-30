using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerifyOTP : ContentPage
    {
        private MediaFile _mediaFile;
        //ServiceViewModel viewModel;
        MenuItem SelectedItem;

        AccountViewModel viewModel;

        public VerifyOTP(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            SelectedItem = mnu;
            Title = mnu.Title;

            AccessSettings acnt = new AccessSettings();           
            string phoneNumber = acnt.UserName;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = AccountViewModel.ActualPhoneNumber;
            }

            viewModel.PhoneNumber = phoneNumber;
            viewModel.GetVerificationAsync();
        }

        private void btnSubmitOtp_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@zamtel.zm", "Skype: zamtel@outlook.com", "Call: +263 787 324 123");
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