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
    public partial class BidDetails : ContentPage
    {
        RequestViewModel viewModel;
        
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }

        public BidDetails(MenuItem selected)
        {
            InitializeComponent();
            BindingContext = viewModel = new RequestViewModel(this);
        }
    }
}