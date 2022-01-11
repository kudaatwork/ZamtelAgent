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
            viewModel.Title = "Promotion Search";
           SelectedItem =selected;
            InitSearch();

        }
        private void searchBar_Focused(object sender, FocusEventArgs e)
        {
            
        }
        private async void filterSearch(string search_text)
        {
            if (string.IsNullOrEmpty(search_text) && search_text.Length >=3)
            {
                SelectedItem.Note = search_text;
                await viewModel.ExecuteGetSearchCommand(SelectedItem);
            }
        }
        private void InitSearch()
        {
            searchBar.TextChanged += (s, e) => filterSearch(searchBar.Text);
            searchBar.SearchButtonPressed += (s, e) => filterSearch(searchBar.Text);
        }
        
    }
}