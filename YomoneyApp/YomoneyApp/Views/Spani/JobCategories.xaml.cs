using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spani
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobCategories : ContentPage
    {
        RequestViewModel viewModel;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public JobCategories(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetStoresCommand.Execute(null);
        }

    }
}