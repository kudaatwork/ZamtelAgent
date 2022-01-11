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
                    Navigation.PushModalAsync(new CreateSpaniProfile(null));
                    Navigation.PopAsync();
                }
            }
            catch
            { }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                    return;

                viewModel.GetOppotunitiesCommand.Execute(null);

                await viewModel.ExecuteGetPopularCategoriesCommand();

                //PickerStore.Items.Clear();
                //foreach (var store in stores)
                //    PickerStore.Items.Add(store.Title.Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ", Inner Exception:" + ex.InnerException + ", Stack Trace:" + ex.StackTrace);
            
            }
            
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // var tx = CategoryName.Text;
            //var Sender = (Button)sender;
           //var title = Sender.Text.Trim();

            var sender2 = (Label)sender;
            var title2 = sender2.Text.Trim();

            //var view = sender as Xamarin.Forms.Button;
            if (Navigation.NavigationStack.Count == 0 ||
            Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
            {
                if (title2 != null)
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(title2));
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(null));
                    await Navigation.PopAsync();
                }
            }
            
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            // var tx = CategoryName.Text;
            //var Sender = (Button)sender;
            //var title = Sender.Text.Trim();

            var sender2 = (Label)sender;
            var title2 = sender2.Text.Trim();

            //var view = sender as Xamarin.Forms.Button;
            if (Navigation.NavigationStack.Count == 0 ||
            Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
            {
                if (title2 != null)
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(title2));
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(null));
                    await Navigation.PopAsync();
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // var tx = CategoryName.Text;
            //var Sender = (Button)sender;
            //var title = Sender.Text.Trim();

            var sender2 = (Label)sender;
            var title2 = sender2.Text.Trim();

            //var view = sender as Xamarin.Forms.Button;
            if (Navigation.NavigationStack.Count == 0 ||
            Navigation.NavigationStack.Last().GetType() != typeof(CreateSpaniProfile))
            {
                if (title2 != null)
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(title2));
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushModalAsync(new CreateSpaniProfile(null));
                    await Navigation.PopAsync();
                }
            }
        }


    }
}