using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.Menus
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectPage : ContentPage
    {
        SelectViewModel viewModel;
        MenuItem SelectedItem;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public SelectPage(MenuItem SelectItem)
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.SelectViewModel(this, SelectItem);
            SelectedItem = SelectItem;
        }

      protected  override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Actions.Count > 0 || viewModel.IsBusy)
                return;

            //viewModel.GetActionCommand.Execute(SelectedItem);
             viewModel.GetSeledtedValues(SelectedItem);
        }
    }
}