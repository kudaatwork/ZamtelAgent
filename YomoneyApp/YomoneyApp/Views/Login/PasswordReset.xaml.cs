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
    public partial class PasswordReset : ContentPage
    {
        AccountViewModel viewModel;

        public PasswordReset(string phone, string email)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = phone;
            viewModel.Email = email;
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
    }
}