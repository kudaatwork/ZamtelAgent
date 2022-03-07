using Newtonsoft.Json;
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
        PromotionsViewModel promotionsViewModel;
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

        public CreateSpaniProfile(string categorySelected)
        {
            InitializeComponent();

            BindingContext = viewModel = new RequestViewModel(this);

            MenuItem menuItem = new MenuItem();
            menuItem.Title = "Create Advert";
            promotionsViewModel = new PromotionsViewModel(this, menuItem);

            if (!string.IsNullOrEmpty(categorySelected))
            {
                viewModel.Category = categorySelected;
            }

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

        //public CreateSpaniProfile(string categorySelected)
        //{
        //    InitializeComponent();

        //    BindingContext = viewModel = new RequestViewModel(this);

        //    if (!string.IsNullOrEmpty(categorySelected))
        //    {
        //        viewModel.Category = categorySelected;
        //    }

        //    PickerStore.SelectedIndexChanged += async (sender, e) =>
        //    {

        //        viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
        //        //viewModel.GetSubcategoriesCommand.Execute(null);
        //        var stores = await viewModel.GetStoreAsync("JobSubCategory");
        //        foreach (var store in stores)
        //            PickerSubcategory.Items.Add(store.Title.Trim());
        //    };

        //    PickerSubcategory.SelectedIndexChanged += (sender, e) =>
        //    {
        //        viewModel.Subcategory = PickerSubcategory.Items[PickerSubcategory.SelectedIndex];
        //    };

        //    ButtonClose.Clicked += async (sender, e) =>
        //    {
        //        await App.Current.MainPage.Navigation.PopModalAsync();
        //        // await Navigation.PopModalAsync();
        //    };

        //    EditorAddress.TextChanged += async (sender, e) =>
        //    {
        //        if (e.NewTextValue.Length > 5)
        //        {
        //            await viewModel.GetGeoAddressAsync(e.NewTextValue);
        //        }
        //    };
        //}

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (!string.IsNullOrEmpty(viewModel.Category))
                {
                    viewModel.HasCategory = true;

                    PickerStore.SelectedItem = viewModel.Category;

                    var stores = await viewModel.GetStoreAsync("JobSubCategory");

                    PickerSubcategory.Items.Clear();

                    foreach (var store in stores)
                        PickerSubcategory.Items.Add(store.Title.Trim());
                }
                else
                {
                    viewModel.HasNoCategory = true;

                    var stores = await viewModel.GetStoreAsync("JobSectors");

                    PickerStore.Items.Clear();

                    foreach (var store in stores)
                        PickerStore.Items.Add(store.Title.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Error!", "Unable to gather billers because of a server error. Contact customer support", "OK");
            }
        }
    }
}