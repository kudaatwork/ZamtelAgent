using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
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
                await App.Current.MainPage.Navigation.PopAsync();
                //await Navigation.PopModalAsync();
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

            }
        }

        private void Share_Clicked(object sender, EventArgs e)
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
    }
}