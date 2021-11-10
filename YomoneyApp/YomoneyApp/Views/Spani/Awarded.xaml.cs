using YomoneyApp.Views.Chat;
using YomoneyApp.Views.Webview;
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
    public partial class Awarded : ContentPage
    {
        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public Awarded(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };

        }

        public void ChatClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                    Navigation.NavigationStack.Last().GetType() != typeof(MassagingPagexaml))
                {
                    Navigation.PushModalAsync(new MassagingPagexaml(view.CommandParameter.ToString()));
                    Navigation.PopAsync();
                }
            }
            catch
            { }
        }

        public void CompleteClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(WebviewPage))
                {
                    Navigation.PushModalAsync(new WebviewPage("http://192.168.100.150:5000/Mobile/JobProfile?id=" + view.CommandParameter.ToString(), "Job Profile", true,null));
                    Navigation.PopAsync();
                }
            }
            catch
            { }
        }
        
        public void RateClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(MassagingPagexaml))
                {
                    viewModel.GetAwardBid(this.Navigation, SelectedItem);
                }
            }
            catch
            { }
        }

        public void PayClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(MassagingPagexaml))
                {
                    viewModel.GetAwardBid(this.Navigation, SelectedItem);
                }
            }
            catch
            { }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetBids(SelectedItem);
        }
    }
}