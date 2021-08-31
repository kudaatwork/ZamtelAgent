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

        public EmailAddress()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);            
        }
    }
}