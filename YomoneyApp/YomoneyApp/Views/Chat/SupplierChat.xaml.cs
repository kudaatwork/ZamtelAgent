using MvvmHelpers;
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
    public partial class SupplierChat : ContentPage
    {
        ChatViewModel viewModel;

        string dbPath = "";

        private ObservableRangeCollection<ChatMessage> _messages;
        public ObservableRangeCollection<ChatMessage> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged("Messages");
            }
        }

        public SupplierChat(string CustomerId)
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.ChatViewModel(this);

            string[] acc = CustomerId.Split('_');
            //itm.SupplierId = itm.SupplierId + "_" + itm.Title + "_" + itm.ServiceId + "_" + "Job" + "_" + itm.Id + "_" + trn.Product;
            //                 ReceiverId             name              agendid                type          bidid         jobid
            if (acc.Length == 6 && acc[2] != "0")
            {
                viewModel.ChatMessage.ReceiverId = acc[0] + "_" + acc[2] + "_" + acc[4] + "_" + acc[5] + "_" + acc[3];
                viewModel.ChatMessage.RequestType = acc[3];
            }
            else
            {
                viewModel.ChatMessage.ReceiverId = acc[0] + "_" + acc[2] + "_" + acc[4] + "_" + acc[5] + "_" + "chat";
            }

            viewModel.Title = acc[1];
            MessageList.ItemAppearing += OnItemAppearing;
            MessageList.ItemDisappearing += OnItemDisappearing;

            //MessageList.ItemAppearing +=  (sender, e) =>
            // {
            //    MessageList.ScrollTo(e.Item, ScrollToPosition.End, false);
            // };
            ButtonClose.Clicked += async (sender, e) =>
            {
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await Navigation.PopModalAsync();
            };            
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //try
            //{
            //    if (e.Item == viewModel.Messages.Last())
            //    {//[viewModel.Messages.Count()-1])
            //        viewModel.MarkReadCommand.Execute(e.Item);
            //        MessageList.ScrollTo(e.Item, ScrollToPosition.End, false);
            //    }
            //    else if (e.Item == viewModel.Messages.First())
            //    {
            //        MessageList.ScrollTo(e.Item, ScrollToPosition.End, false);
            //    }
            //}
            //catch { };

        }

        private void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item == viewModel.Messages.Last())
                IsBusy = false;
        }
    }
}