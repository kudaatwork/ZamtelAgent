using YomoneyApp.Views.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.ViewModels.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignIn : ContentPage
    {
        AccountViewModel viewModel;
        public SignIn()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            
        }

        async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ForgotPasswordOptionPage());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAccount());
        }
    }
}