using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Views.Fileuploads;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordOptionPage : ContentPage
    {
        public ForgotPasswordOptionPage()
        {
            InitializeComponent();
        }

        private async void btnEmailOption_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EmailVerificationPage());
        }

        private async void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PictureFileUpload());
        }
    }
}