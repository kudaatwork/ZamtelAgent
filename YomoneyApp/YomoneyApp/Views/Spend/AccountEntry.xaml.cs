using Newtonsoft.Json;
using RetailKing.Models;
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
    public partial class AccountEntry : ContentPage
    {
        SpendViewModel viewModel;
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public AccountEntry(MenuItem trn)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);
            var servics = JsonConvert.DeserializeObject<List<MenuItem>>(trn.Section);
            var service = servics.FirstOrDefault();
            viewModel.Budget = trn.Amount.ToString();
            viewModel.Category = service.ServiceId.ToString();
            viewModel.Title = service.Title;
            viewModel.JobPostId = service.Description;
            if (trn.Note == "Reward Service")
            {
                viewModel.Description = "Reward";
                viewModel.Ptitle = "You have redeemed " + service.Title + " worth $" + trn.Amount + " please enter you " + service.Title + " account number";
            }
        }
    }
}