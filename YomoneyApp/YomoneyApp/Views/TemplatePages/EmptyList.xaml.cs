using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.TemplatePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyList : ContentPage
    {
        public EmptyList(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = selected;

            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };
        }
    }
}