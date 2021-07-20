using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Spend
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TockenPage : ContentPage
    {
        SpendViewModel viewModel;
        List<MenuItem> tokens;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public TockenPage(List<MenuItem> menus)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);
            viewModel.Stores.ReplaceRange(menus);
            tokens = menus;
            BindingContext = viewModel;
        }

        public void ShareClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                foreach (var tkn in tokens)
                {
                    viewModel.GetShareToken(tkn);
                }
            }
            catch
            {
            }

        }

    }
}