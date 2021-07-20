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
    public partial class HistoryList : ContentPage
    {
        TransactionViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public HistoryList(MenuItem selected)
        {
            selected.Title = "Transactions";
            InitializeComponent();
            BindingContext = viewModel = new TransactionViewModel(this, selected);
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Transactions.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetHistoryCommand.Execute(null);
        }
    }
}