using Newtonsoft.Json;
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
    public partial class PromotionSearch : ContentPage
    {

        PromotionsViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public PromotionSearch(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new PromotionsViewModel(this, selected);
           SelectedItem =selected;

        }
        private void searchBar_Focused(object sender, FocusEventArgs e)
        {
            
        }
        private void text_Changed(object sender, FocusEventArgs e)
        {
            var view = sender as Xamarin.Forms.SearchBar;
            
            SelectedItem.Note = view.SearchCommandParameter.ToString();
            viewModel.GetSearchPromotions(SelectedItem);
        }
        
    }
}