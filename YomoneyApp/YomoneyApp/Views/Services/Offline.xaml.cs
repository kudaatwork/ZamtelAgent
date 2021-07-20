using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Offline : ContentPage
    {
        public Offline()
        {
            InitializeComponent();
            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };
        }
    }
}