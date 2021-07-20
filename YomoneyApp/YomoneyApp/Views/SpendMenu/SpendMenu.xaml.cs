using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.SpendMenu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpendMenu : ContentPage
    {
        SpendViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public SpendMenu()
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetMenuCommand.Execute(null);
        }
    }
}