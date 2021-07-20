using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Loyalty
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JoinLoyalty : ContentPage
    {
        LoyaltyViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public JoinLoyalty(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new LoyaltyViewModel(this, selected);
            SelectedItem = selected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Stores.Count > 0 || viewModel.IsBusy)
                return;

            viewModel.GetSelectedSupplier(SelectedItem);
        }
    }
}