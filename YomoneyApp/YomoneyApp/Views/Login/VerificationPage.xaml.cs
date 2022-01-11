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
            //await Navigation.PushAsync(new NewAccount());
            viewModel.GetVerificationAsync();
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