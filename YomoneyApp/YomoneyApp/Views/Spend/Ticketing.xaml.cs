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
    public partial class Ticketing : ContentPage
    {

        SpendViewModel viewModel;
        MenuItem SelectedItem;

        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public Ticketing(List<MenuItem> menus)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);
        }

    }
}