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
    public partial class EmailAddress : ContentPage
    {
        AccountViewModel viewModel;

        public EmailAddress(string phone)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = phone;
        }

        private void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
        }
    }
}