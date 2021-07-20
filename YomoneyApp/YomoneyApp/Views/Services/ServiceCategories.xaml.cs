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
    public partial class ServiceCategories : ContentPage
    {
        
        ServiceViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }


        public ServiceCategories(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, selected);
            SelectedItem = selected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetServicesCategories(SelectedItem);
        }
    }
}