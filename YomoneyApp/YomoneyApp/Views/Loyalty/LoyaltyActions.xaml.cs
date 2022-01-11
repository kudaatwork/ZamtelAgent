using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Profile.Loyalty
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoyaltyActions : ContentPage
    {
        LoyaltyViewModel viewModel;
        MenuItem SelectedItem;
       
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public LoyaltyActions(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new LoyaltyViewModel(this, selected);
            SelectedItem = selected;
           
            ButtonClose.Clicked += async (sender, e) =>
            {
                for (var counter = 1; counter < 2; counter++)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
                await Navigation.PopAsync();
            };
        }
      
        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            for (var counter = 1; counter < 2; counter++)
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetSelectedSupplier(SelectedItem);
        }
    }
}