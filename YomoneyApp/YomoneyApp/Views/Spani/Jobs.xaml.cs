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
    public partial class Jobs : ContentPage
    {
        RequestViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public Jobs()
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
        }

        public async void RemoveClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();

                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

               await viewModel.ExecuteCancelJobRequestCommand(mn);            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void JobsClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();

                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                if (Navigation.NavigationStack.Count == 0 || Navigation.NavigationStack.Last().GetType() != typeof(RequestBids))
                {
                    Navigation.PushModalAsync(new RequestBids(mn));
                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

                if (Navigation.NavigationStack.Count == 0 || Navigation.NavigationStack.Last().GetType() != typeof(Awarded))
                {
                    // Navigation.PushModalAsync(new Awarded(mn));
                    // Navigation.PopAsync();

                    string link = "https://www.yomoneyservice.com/Mobile/Projects?Id=" + uname;

                    await Navigation.PushModalAsync(new WebviewHyubridConfirm(link, "Awarded Jobs", true, "#22b24c"));

                    //await Navigation.PushAsync(new WebviewHyubridConfirm(link, "Awarded Jobs", true, "#22b24c"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

                if (Navigation.NavigationStack.Count == 0 || Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
                {
                    Navigation.PushModalAsync(new PlaceBid(mn));
                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void newSkillClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();

                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                if (Navigation.NavigationStack.Count == 0 || Navigation.NavigationStack.Last().GetType() != typeof(JobRequest))
                {
                    Navigation.PushModalAsync(new JobRequest());
                    Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetJobListCommand.Execute(null);
        }
    }
}