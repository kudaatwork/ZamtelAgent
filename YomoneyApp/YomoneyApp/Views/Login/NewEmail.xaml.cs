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
    public partial class NewEmail : ContentPage
    {
        AccountViewModel viewModel;
        public NewEmail(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = mnu.Note;
            viewModel.Subtitle = mnu.Description;
            viewModel.Email = mnu.Title;
        }
    }
}