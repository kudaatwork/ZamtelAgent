using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;

namespace YomoneyApp.ViewModels.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewAccount : ContentPage
    {
        AccountViewModel viewModel;
        CountryPickerViewModel countryPickerViewModel;
        public NewAccount()
        {
            InitializeComponent();

            //BindingContext = countryPickerViewModel = new CountryPickerViewModel();
            BindingContext = viewModel = new AccountViewModel(this);
            countryPickerViewModel = new CountryPickerViewModel(this);

            Gender.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.FormGender = Gender.Items[Gender.SelectedIndex];
            };
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //Navigation.PopAsync();
            await Navigation.PushAsync(new SignIn());
            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //viewModel.SelectedCountry = countryPickerViewModel.SelectedCountry;

             await viewModel.ExecuteGetCurrentGeolocationCommand();

            var genderOptions = new List<string> { "Male", "Female" };

            if (string.IsNullOrEmpty(viewModel.FormGender))
            {
                Gender.Items.Clear();
                foreach (var genderOption in genderOptions)
                    Gender.Items.Add(genderOption);
            }
        }

        private void ButtonSubmitFeedback_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = true;
            }
        }
    }
}