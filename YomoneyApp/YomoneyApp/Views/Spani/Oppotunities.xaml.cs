using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
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
    public partial class Oppotunities : ContentPage
    {
        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
       
        public Oppotunities()
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
        }
        
        public void PlaceClicked(object sender, EventArgs e)
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
                    if (mn.SupplierId == "Share Post")
                    {
                        if (CrossShare.Current.SupportsClipboard)
                        {
                            CrossShare.Current.Share(new ShareMessage
                            {
                                Title = "YoApp Oppotunity",
                                Text = "YoApp Job Leads" + "\r\n\r\n" +  mn.Description + "\r\n\r\n " + " To receive these Job Oppotunities download Yoapp " + "\r\n\r\n" + "https://play.google.com/store/apps/details?id=com.Faithwork.Yomoney",
                                Url = ""
                            });
                        }
                        else
                        {
                            return;
                            // DependencyService.Get<IClipBoard>().OnCopy(selectedToCopy.Description);
                        }
                    }
                    else
                    {
                        Navigation.PushModalAsync(new PlaceBid(mn));
                        Navigation.PopAsync();
                    }
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

        public void newSkill(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                if (Navigation.NavigationStack.Count == 0 ||
                Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
                {
                    Navigation.PushModalAsync(new CreateSpaniProfile());
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

            viewModel.GetOppotunitiesCommand.Execute(null);
            
        }
    }
}