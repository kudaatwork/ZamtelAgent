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

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SharePage : ContentPage
    {
        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public SharePage(MenuItem menu)
        {
            InitializeComponent();
            SelectedItem = menu;
            BindingContext = menu;
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
            catch
            { }
        }


        public void ClickCommand(object sender, EventArgs e)
        {
            try
            {
                string url = SelectedItem.WebLink;
                Device.OpenUri(new System.Uri(url));
            }
            catch
            { }
        }

    }
}