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
    public partial class ServiceBilling : ContentPage
    {

        SpendViewModel viewModel;
        List<MenuItem> itm = new List<MenuItem>();
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public ServiceBilling(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);
            viewModel.RetryText = "Retry";
            if (mnu.Note == "Reward Service")
            {
                viewModel.Budget = mnu.Amount;
                viewModel.Retry = true;
                viewModel.IsConfirm = false;
                viewModel.RetryText = "Get Reward";
            }
            viewModel.Title = mnu.Title.Trim() + " Payment";
            viewModel.Category = mnu.Title;
            viewModel.Categories.Add(mnu);

            if(mnu.TransactionType == 3)
            {
                viewModel.RequireAccount = true;
            }

            if (mnu.HasProducts)
            {
                viewModel.IsConfirm = false;
                viewModel.HasProducts = true;
            }
            else
            {
                viewModel.IsConfirm = true;
                viewModel.HasProducts = false;
            }
            
            PickerProducts.SelectedIndexChanged +=  (sender, e) =>
            {
                string Product = PickerProducts.Items[PickerProducts.SelectedIndex];
                string[] dr = Product.Split('$');
                var title = dr[0];
                var biller = viewModel.Denominations.Where(u => u.Title.Trim() == title.Trim()).FirstOrDefault();
                viewModel.Budget = biller.Amount;
                viewModel.Ptitle = Product;
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(viewModel.Category) || !viewModel.IsBusy)
            {
                try
                {
                    var stores = await viewModel.GetProductsAsync();
                    foreach (var store in stores)
                        PickerProducts.Items.Add(store.Title.Trim() + " $" + store.Amount);
                }
                catch (Exception ex)
                {

                }

            }
        }
    }
}