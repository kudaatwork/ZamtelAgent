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

            ButtonForgot.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new ForgotPassword());
            };
        }

    }
}