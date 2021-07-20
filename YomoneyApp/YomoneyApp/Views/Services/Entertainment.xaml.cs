using MediaManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Entertainment : ContentPage
    {
        ServiceViewModel viewModel;
        MenuItem SelectedItem;
        List<string> Mp3UrlList = new List<string>();
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public  Entertainment(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, selected);
            SelectedItem = selected;
            foreach (var itm in viewModel.ServiceOptions)
            {
                if (itm.Section == "AUDIO")
                {
                    Mp3UrlList.Add(itm.WebLink);
                }
            }
            if (Mp3UrlList.Count > 0)
            {
                CrossMediaManager.Current.Play(Mp3UrlList);
            }
        }
        private async void Download_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);
                //await Navigation.PushAsync(new AmountPopup(mn));

                await Navigation.PushModalAsync(new PaymentPage(mn));

                IsBusy = false;
                MessagingCenter.Subscribe<string, string>("PaymentRequest", "NotifyMsg", async (rsender, arg) =>
                {
                    if (arg == "Payment Success")
                    {
                        viewModel.SaveFileCommand.Execute(mn);
                    }
                    else
                    {
                        await DisplayAlert("Payment Failed", "Unable to buy make sure you do the payment correctly.", "OK");
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }
              
        public async void lvItemTapped(object sender, ItemTappedEventArgs e)
        {
            var myListView = (ListView)sender;
            
            var x = JsonConvert.SerializeObject(myListView.SelectedItem);
            var mn = JsonConvert.DeserializeObject<MenuItem>(x);
           
            if (mn.Section == "AUDIO")
            {
                mn.IsShare = true;

                if (Mp3UrlList.Count() > 0)
                {
                    var indx = Mp3UrlList.IndexOf(mn.WebLink);
                    await CrossMediaManager.Current.PlayQueueItem(indx);
                }
                else
                {
                    await CrossMediaManager.Current.Play(mn.WebLink);
                }
            }
            else if (mn.Section == "VIDEO")
            {
              await Navigation.PushAsync(new VideoAudioPlayer(mn));
            }
            myListView.SelectedItem = null;
        }
      
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetMediaFile(SelectedItem);
        }
    }
}