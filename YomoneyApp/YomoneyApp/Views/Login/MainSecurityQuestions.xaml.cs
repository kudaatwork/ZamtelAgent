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
    public partial class MainSecurityQuestions : ContentPage
    {
        AccountViewModel viewModel;

        MenuItem mn = new MenuItem();

        public MainSecurityQuestions(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = mnu.Note;
            viewModel.Subtitle = mnu.Description;
            viewModel.Email = mnu.Title;
        }

        private async void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SecurityQuestion());
        }
    }
}