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
    public partial class AddEmailAddress : ContentPage
    {
        AccountViewModel viewModel;
               
        public AddEmailAddress(string phone)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.ActualPhoneNumber = phone;
        }

       /* private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (viewModel.PhoneNumber != null)
                {
                    viewModel.LoadQuestions();
                }
                else
                {
                    await DisplayAlert("PhoneNumber Error", "There has been an error in verifying who you are", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("PhoneNumber Error", ex.Message, "OK");
            }            
        }*/

        private async void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (viewModel.PhoneNumber != null)
                {
                    viewModel.LoadQuestions();
                }
                else
                {
                    await DisplayAlert("PhoneNumber Error", "There has been an error in verifying who you are", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("PhoneNumber Error", ex.Message, "OK");
            }
        }

        private void btnSecurityQtnOption_Clicked_1(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }
    }
}