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
    }
}