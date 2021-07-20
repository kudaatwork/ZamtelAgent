using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YomoneyApp.Views.TemplatePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmPage : ContentPage
    {
        SpendViewModel viewModel;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public ConfirmPage(TransactionRequest mn)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);

            viewModel.ServiceId = mn.ServiceId;
            viewModel.Currency = mn.Currency;
            viewModel.Budget = mn.Amount.ToString();
            viewModel.AccountNumber = mn.CustomerMSISDN;
            viewModel.PhoneNumber = mn.Source;
            viewModel.Email = mn.ServiceProvider;
            viewModel.JobPostId = mn.Product;
            viewModel.Ptitle = mn.CustomerData;
            viewModel.Category = mn.Narrative;
            viewModel.RetryText = "Retry";
            if (mn.Note == "Reward Service")
            {
                viewModel.IsConfirm = false;
                viewModel.Retry = true;
                viewModel.RetryText = "Get Reward";
            }

            viewModel.Title = "Confirm Details";
            MenuItem mnu = new YomoneyApp.MenuItem();
            mnu.Description = mn.CustomerData;
            viewModel.Stores.Add(mnu);

        }

        public void ShareClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                foreach (var tkn in viewModel.Stores)
                {
                    viewModel.GetShareToken(tkn);
                }
            }
            catch
            {
            }

        }

        public void ConfirmClicked(object sender, EventArgs e)
        {
            try
            {
                var view = sender as Xamarin.Forms.Button;
                viewModel.GetTokenCommand.Execute(null);
            }
            catch
            { }

        }
        public void CancelClicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch
            { }
        }
    }
}