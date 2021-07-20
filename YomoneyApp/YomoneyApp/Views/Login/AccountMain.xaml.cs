using YomoneyApp.ViewModels.Login;
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
    public partial class AccountMain : ContentPage
    {
        AccountViewModel viewModel;
        public AccountMain()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);

            ButtonJoin.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new NewAccount());
            };

            ButtonLogin.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new SignIn());
            };
        }
    }
}