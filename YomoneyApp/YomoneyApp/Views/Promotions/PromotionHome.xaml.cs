using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
//using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Promotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromotionHome : ContentPage
    {

        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public PromotionHome(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new PromotionsViewModel(this, selected);
            SelectedItem = selected;

            ButtonClose.Clicked += async (sender, e) =>
            {
                //await App.Current.MainPage.Navigation.PopAsync();
                //await Navigation.PopModalAsync();
               //MediaElement.AutoPlayProperty = false;
                await Navigation.PushAsync(new HomePage());
            };

            ButtonSearch.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new PromotionSearch(SelectedItem));
            };
        }

        private void Like_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                ServiceViewModel svm = new ServiceViewModel(this, mn);
                svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Share_Clicked(object sender, EventArgs e)
        {
            try
            {
                /*var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                ServiceViewModel svm = new ServiceViewModel(this, mn);
                svm.RenderServiceAction(mn);*/

                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                //mn.Id = view.CommandParameter.ToString();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                if (CrossShare.Current.SupportsClipboard)
                {
                    CrossShare.Current.Share(new ShareMessage
                    {
                        Title = mn.Title,
                        Text = mn.Description,
                        Url = mn.WebLink,
                    });
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Learn_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                ServiceViewModel svm = new ServiceViewModel(this, mn);
                svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
               

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
            viewModel.GetCurrentPromotions(SelectedItem);
        }

        private void MediaElement_MediaFailed(object sender, EventArgs e)
        {

        }

        private async void Button_Clicked(object sender, EventArgs e) // Post Lead
        {
            viewModel.IsBusy = true;
            viewModel.Message = "Loading";

            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem menuItem = new YomoneyApp.MenuItem();
                var menu = JsonConvert.SerializeObject(view.CommandParameter);
                menuItem = JsonConvert.DeserializeObject<MenuItem>(menu);

                PromotionsViewModel promotion = new PromotionsViewModel(this, menuItem);
                promotion.GetLeads(menuItem);

                viewModel.IsBusy = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await DisplayAlert("Leads Error!", "Unable to gather leads from the server. Please check your internet connection and try again", "Ok");
            }
        }
    }
}