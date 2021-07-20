using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateSpaniProfile : ContentPage
    {
        RequestViewModel viewModel;
        string googleurl = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input=Museum%20of%20Contemporary%20Art%20Australia&inputtype=textquery&fields=formatted_address,name,rating,opening_hours,geometry&key=AIzaSyDWd6uSjjohKOWKMa5tefGw30Uk3CkbeJ0";
        public CreateSpaniProfile()
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);

            PickerStore.SelectedIndexChanged += async (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
                //viewModel.GetSubcategoriesCommand.Execute(null);
                var stores = await viewModel.GetStoreAsync("JobSubCategory");
                foreach (var store in stores)
                    PickerSubcategory.Items.Add(store.Title.Trim());
            };


            PickerSubcategory.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Subcategory = PickerSubcategory.Items[PickerSubcategory.SelectedIndex];
            };
            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
               // await Navigation.PopModalAsync();
            };
            EditorAddress.TextChanged += async (sender, e) =>
            {
                if (e.NewTextValue.Length > 5)
                {
                     await viewModel.GetGeoAddressAsync(e.NewTextValue);
                    
                }
            };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
       
            try
            {
                var stores = await viewModel.GetStoreAsync("JobSectors");
                PickerStore.Items.Clear();
                foreach (var store in stores)
                    PickerStore.Items.Add(store.Title.Trim());
            }
            catch (Exception ex)
            {
             
            }
            

        }
    }
}