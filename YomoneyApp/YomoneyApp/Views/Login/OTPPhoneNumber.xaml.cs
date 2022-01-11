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
    public partial class OTPPhoneNumber : ContentPage
    {
        AccountViewModel viewModel;
        public OTPPhoneNumber()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
        }

        private void ButtonSignIn_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }
    }
}