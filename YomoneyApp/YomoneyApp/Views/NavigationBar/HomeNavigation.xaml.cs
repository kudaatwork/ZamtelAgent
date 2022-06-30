using YomoneyApp.Views.Chat;
using YomoneyApp.Views.QRScan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;

namespace YomoneyApp.Views.NavigationBar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeNavigation : ContentView
    {
        ChatViewModel viewModel;
        
        public HomeNavigation()
        {
            InitializeComponent();
            Page nav = new Page();
            BindingContext = viewModel= new YomoneyApp.ChatViewModel(nav);// ViewModelLocator.ChatMainViewModel;
           
            // viewModel.GetUnreadCountCommand.Execute(null);
        }
      
        private void Tapchart_OnTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChatTabs());
        }

        private void Tapqr_OnTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new QRScanPage());
        }

    }
}