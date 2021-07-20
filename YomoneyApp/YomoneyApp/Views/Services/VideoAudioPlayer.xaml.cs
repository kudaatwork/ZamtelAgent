using MediaManager;
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
    public partial class VideoAudioPlayer : ContentPage
    {
        ServiceViewModel viewModel;
        MenuItem SelectedItem;
       
       
        public VideoAudioPlayer(MenuItem mn)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, mn);
            
           // CrossMediaManager.Current.Play(mn.WebLink);
        }
     
    }
}