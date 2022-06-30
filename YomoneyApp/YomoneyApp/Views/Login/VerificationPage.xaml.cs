using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Login;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerificationPage : ContentPage
    {
        AccountViewModel viewModel;
        public VerificationPage(string phone)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = phone;
            viewModel.GetVerificationAsync();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var resendCode = await DisplayAlert("Alert!", "Do you really want to resend your verification code?", "Yes", "No");

            if (resendCode)
            {
                viewModel.GetVerificationAsync();
            }
        }

        private void ButtonSignIn_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;                
            }
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");
        }
    }
}