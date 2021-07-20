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

      
    }
}