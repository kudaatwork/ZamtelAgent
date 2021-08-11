using Newtonsoft.Json;
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
    public partial class PayBill : ContentPage
    {
        SpendViewModel viewModel;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public PayBill(MenuItem mnu)
        {
            InitializeComponent();

            BindingContext = viewModel = new SpendViewModel(this);
            viewModel.RetryText = "Retry";
            viewModel.Currency = mnu.Currency;

            if (mnu.TransactionType == 3)
            {
                viewModel.RequireAccount = true;
            }

            if (mnu.Note == "Reward Service")
            {
                viewModel.Budget = mnu.Amount;
                viewModel.Retry = true;
                viewModel.IsConfirm = false;
                viewModel.RetryText = "Get Reward";
            }

            PickerStore.SelectedIndexChanged += async (sender, e) =>
            {
                viewModel.Category = PickerStore.Items[PickerStore.SelectedIndex];
                var biller = viewModel.Categories.Where(u => u.Title.Trim() == viewModel.Category.Trim()).FirstOrDefault();

                if (biller.HasProducts)
                {
                    viewModel.IsConfirm = false;
                    viewModel.HasProducts = true;
                    var stores = await viewModel.GetProductsAsync();
                    foreach (var store in stores)
                        PickerProducts.Items.Add(store.Title.Trim() + " $" + store.Amount);
                }
                else
                {
                    viewModel.IsConfirm = true;
                    viewModel.HasProducts = false;
                }
            };

            PickerProducts.SelectedIndexChanged += (sender, e) =>
            {
                string Product = PickerProducts.Items[PickerStore.SelectedIndex];
                string[] dr = Product.Split('$');
                var title = dr[0];
                var biller = viewModel.Denominations.Where(u => u.Title.Trim() == title.Trim()).FirstOrDefault();
                viewModel.Budget = biller.Amount;
                viewModel.Ptitle = Product;
            };
        }
        private async void Section_Clicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;

                MenuItem mn = new YomoneyApp.MenuItem();

                var x = JsonConvert.SerializeObject(view.CommandParameter);

                mn = JsonConvert.DeserializeObject<MenuItem>(x);

                viewModel.Category = mn.Title;

                var biller = viewModel.Categories.Where(u => u.Title.Trim() == viewModel.Category.Trim()).FirstOrDefault();

                if (biller.HasProducts)
                {
                    viewModel.IsConfirm = false;
                    viewModel.HasProducts = true;
                    var stores = await viewModel.GetProductsAsync();

                    foreach (var store in stores)
                        PickerProducts.Items.Add(store.Title.Trim() + " $" + store.Amount);
                }
                else
                {
                    viewModel.IsConfirm = true;
                    viewModel.HasProducts = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if ((viewModel.Category != null && viewModel.Category != "") || viewModel.IsBusy)
            {

            }
            else
            {
                try
                {
                    var stores = await viewModel.GetStoreAsync("JobSectors");
                    PickerStore.Items.Clear();
                    foreach (var store in stores)
                        PickerStore.Items.Add(store.Title.Trim());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}