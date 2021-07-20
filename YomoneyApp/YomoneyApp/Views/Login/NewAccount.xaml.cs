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
    public partial class NewAccount : ContentPage
    {
        AccountViewModel viewModel;
        public NewAccount()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
        }
    }
}