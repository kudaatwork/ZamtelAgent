using YomoneyApp.Views.Chat;
using YomoneyApp.Views.Webview;
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
    public partial class RequestBids : ContentPage
    {
        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public RequestBids(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
            //var x = JsonConvert.SerializeObject(selected);
            //viewModel.Category = x;
            SelectedItem = selected;
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

      
        public void ProfileClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(WebviewPage))
                {      
                   Navigation.PushModalAsync(new WebviewPage("http://192.168.100.150:5000/Mobile/JobProfile?id="+ view.CommandParameter.ToString(), "Job Profile",true,null));
                   Navigation.PopAsync();
                }
            }
            catch
            { }
        }

        public void AwardClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                //mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(MassagingPagexaml))
                {
                    viewModel.GetAwardBid(this.Navigation, mn);
                }
            }
            catch
            { }
        }

        public void AdvertClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                //mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
                {
                    Navigation.PushModalAsync(new PlaceBid(mn));
                    Navigation.PopAsync();
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