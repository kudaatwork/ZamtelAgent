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
    public partial class ForgotPassword : ContentPage
    {
        AccountViewModel viewModel;
        public ForgotPassword()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
        }
    }
}