using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.TransactionHistory
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeoutList : ContentPage
    {
        TransactionViewModel  viewModel;
        MenuItem SelectedItem;
        public TimeoutList()
        {
            MenuItem mn = new MenuItem { Title = "Transaction History", Description = "View your spending history", Image = "Paymenu.png", Section = "Yomoney", ServiceId = 7, SupplierId = "All", TransactionType = 3 }; ;
            InitializeComponent();
            BindingContext = viewModel = new TransactionViewModel(this,mn);
            SelectedItem = mn;
        }

        public void RetryClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
             
                MenuItem mn = new YomoneyApp.MenuItem();
                SpendViewModel sp = new SpendViewModel(this);
                //sp.GetSuspensePickerAsync(SelectedItem);
            }
            catch
            { }
        }
       
         protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Transactions.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetPendingCommand.Execute(SelectedItem);
        }
    }
}