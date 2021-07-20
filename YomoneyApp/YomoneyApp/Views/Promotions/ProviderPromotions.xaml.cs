using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Promotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProviderPromotions : ContentPage
    {

        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public ProviderPromotions(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new PromotionsViewModel(this, selected);
            SelectedItem = selected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetSelectedProvider(SelectedItem);
        }
    }
}
