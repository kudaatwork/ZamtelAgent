using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaletServices : ContentPage
    {
        WalletServicesViewModel viewModel;        

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public WaletServices()
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.WalletServicesViewModel(this);
            Title = "Dashboard";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetStoresCommand.Execute(null);
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }
    }
}