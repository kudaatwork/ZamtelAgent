using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobBids : ContentPage
    {
        RequestViewModel viewModel;
        MenuItem SelectedItem;
       
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public JobBids()
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
        }
        public void MyQuotesClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
               // mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
                {
                    Navigation.PushModalAsync(new QuoteDetails(mn));
                    Navigation.PopAsync();
                }
            }
            catch
            { }
        }

        public async void AwardedClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                // mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);
                AccessSettings acnt = new AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(Awarded))
                {
                    // Navigation.PushModalAsync(new Awarded(mn));
                    // Navigation.PopAsync();
                    string link = "http://192.168.100.150:5000/Mobile/Projects?Id=" + uname;
                    await Navigation.PushModalAsync(new WebviewHyubridConfirm(link, "Awarded Jobs", true, "#22b24c"));
               
                }
            }
            catch
            { }
        }

        public async void AdvertClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem itm = new YomoneyApp.MenuItem();
                //mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                itm = JsonConvert.DeserializeObject<MenuItem>(x);
                AccessSettings acnt = new AccessSettings();
               string pass = acnt.Password;
                string uname = acnt.UserName;
                if (!string.IsNullOrEmpty(itm.WebLink))
                {
                    if (itm.WebLink.Contains("/Mobile/Forms") || itm.WebLink.Contains("/Mobile//Forms"))
                    {
                        itm.WebLink = itm.WebLink.Trim() + "&Customer=" + uname;
                    }
                    await Navigation.PushAsync(new WebviewPage(itm.WebLink, itm.Title, false, itm.ThemeColor));
                    return;
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

            viewModel.GetQuotedCommand.Execute(null);
        }
    }
}