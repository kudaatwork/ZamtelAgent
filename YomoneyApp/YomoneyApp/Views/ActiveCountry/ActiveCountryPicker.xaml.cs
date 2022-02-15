using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;

namespace YomoneyApp.Views.ActiveCountry
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveCountryPicker : ContentPage
    {
        CountryPickerViewModel viewModel;

        public ActiveCountryPicker()
        {
            InitializeComponent();
            BindingContext = viewModel = new CountryPickerViewModel(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.ExecuteGetCurrentActiveCountryCommand();
        }

        private void ButtonSignIn_Clicked(object sender, EventArgs e)
        {

        }
    }
}