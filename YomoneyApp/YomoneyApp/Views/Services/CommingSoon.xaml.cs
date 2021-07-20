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
    public partial class CommingSoon : ContentPage
    {
        public CommingSoon()
        {
            InitializeComponent();

            ButtonClose.Clicked += async (sender, e) =>
            {
                //await Navigation.PopModalAsync();
                await App.Current.MainPage.Navigation.PopModalAsync();
            };
        }
    }
}