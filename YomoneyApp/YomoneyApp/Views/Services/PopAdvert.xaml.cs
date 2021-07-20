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
    public partial class PopAdvert : ContentView
    {
        HomeViewModel viewModel;
        Page nav = new Page();
        public  PopAdvert()
        {
            
            InitializeComponent();
            BindingContext = viewModel = new HomeViewModel(nav);

            Appearing();
        }
        private void Advert_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                MenuItem mn = new YomoneyApp.MenuItem();
                var x = JsonConvert.SerializeObject(view.CommandParameter);
                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                ServiceViewModel svm = new ServiceViewModel(nav, mn);
                svm.RenderServiceAction(mn);
            }
            catch (Exception ex)
            {

            }
        }
        protected async void Appearing()
        {
            
            await viewModel.ExecuteGetStoresCommand();// viewModel.GetAccountCommand.Execute(null);
           
        }
    }
}