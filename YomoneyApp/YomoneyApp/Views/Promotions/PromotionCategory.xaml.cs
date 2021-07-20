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
    public partial class PromotionCategory : ContentView
    {
   
        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }


        public PromotionCategory()
        {
            InitializeComponent();
            Page pg = new Page();
            MenuItem mn = new MenuItem();
            mn.Title = "PROMOTIONS";
            mn.TransactionType = 23;
            mn.Section = "PROMOTIONS";
            mn.SupplierId = "All";
            BindingContext = viewModel = new PromotionsViewModel(pg, mn);
            SelectedItem = mn;
            if (viewModel.ServiceOptions.Count > 0 || viewModel.IsBusy)
                return;
            viewModel.GetPromotionCategories(SelectedItem);
        }

        
    }
}