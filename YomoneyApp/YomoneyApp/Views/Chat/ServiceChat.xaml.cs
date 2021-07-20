using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceChat : ContentPage
    {
        ChatViewModel viewModel;
        public ServiceChat()
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.ChatViewModel(this);// ViewModelLocator.ChatMainViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Messages.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetSupportCommand.Execute(null);
        }
    }
}