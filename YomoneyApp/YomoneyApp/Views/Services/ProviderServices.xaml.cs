using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProviderServices : ContentPage
    {
        ServiceViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public ProviderServices(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, selected);
            SelectedItem = selected;

            var signOut = new ToolbarItem
            {
                Command = new Command(() =>
                {
                    MenuItem px = new YomoneyApp.MenuItem();

                    px.Title = "Sign Out";
                    AccessSettings ac = new AccessSettings();

                    if (string.IsNullOrEmpty(ac.UserName) || string.IsNullOrEmpty(ac.Password))
                    {
                        ac.DeleteCredentials();

                        // Navigation.PopAsync();

                        Navigation.PushAsync(new AccountMain());
                    }
                    else
                    {
                        Navigation.PushAsync(new AccountMain());
                    }
                   
                }),

                Text = "Sign Out",
                Priority = 0,
                Order = ToolbarItemOrder.Secondary
            };

            ToolbarItems.Add(signOut);            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetSeledtedProvider(SelectedItem);                   
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    base.OnBackButtonPressed();

        //    return true;
        //}
    }
}