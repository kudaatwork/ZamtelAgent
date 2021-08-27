using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Services;

namespace YomoneyApp.Views.TemplatePages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebConfirmPage : ContentPage
    {
        MenuItem mnu;
        SpendViewModel viewModel;
        string url = "";
        bool Error = false;
        string account = "";
        string uname = "";
        public Action<MenuItem> ItemSelected
        {
            get { return viewModel.ItemSelected; }
            set { viewModel.ItemSelected = value; }
        }
        public WebConfirmPage(string source, string title, bool isModal, TransactionRequest mn)
        {
            InitializeComponent();
            BindingContext = viewModel = new SpendViewModel(this);

            viewModel.ServiceId = mn.ServiceId;
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
            
            mnu = new MenuItem();
            mnu.HasProducts = true;
            mnu.Title = title;
            mnu.IsAdvert = false;
            if (isModal) // used to determin if navigation should show
            {
                mnu.IsAdvert = true;
            }
            //BindingContext = mnu; 
            viewModel.Stores.Add(mnu);
            viewModel.IsConfirmed = false;
            viewModel.IsNotBusy = false;
            //ConfirmWebview.AddJavascriptInterface(new JSBridge(this), "CSharp");
            url = source;
            ConfirmWebview.Source = source;
            
            MessagingCenter.Subscribe<string, string>("ConfirmWebview", "ChangeVMState", async (sender, arg) =>
            {
                if (arg == "Confirmed")
                {
                    viewModel.IsConfirmed = true;
                }
            });
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
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await progress.ProgressTo(0.9, 900, Easing.SpringIn);
            try
            {
                var client = new HttpClient();
                string result = await client.GetStringAsync(url);
                if (result != "System.IO.MemoryStream")
                {
                    viewModel.Error404 = false;
                }
            }
            catch
            {
                viewModel.Error404 = true;
            }
        }
        void webOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            progress.IsVisible = true;
            viewModel.Message = "Loading Page...";
           
        }
        void webOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            progress.IsVisible = false;
            AccessSettings acnt = new AccessSettings();
            uname = acnt.UserName;
            var conn = new YomoneyApp.ChatViewModel(this);
            //if(viewModel.Title ==)
            string js = "var mel = document.querySelectorAll('.pay');" +
                         " var el = mel[0];" +
                         " var node, nodes = [];" +
                         " do" +
                        "{" +
                          " var parent = el.parentNode;" +
                          " for (var i = 0, iLen = parent.childNodes.length; i < iLen; i++)" +
                          " {" +
                              " node = parent.childNodes[i];" +

                               "if (node.nodeType == 1 && node != el)" +
                              " {" +
                                 "  nodes.push(node);" +
                               "}" +
                           "}" +
                          " el = parent;" +
                       "} while (el.tagName.toLowerCase() != 'body');" +
                       "nodes.forEach(function(node){" +
                           "node.style.display = 'none';" +
                       "}); " +
                        "document.getElementById('txtSmartCard').value = '" + viewModel.AccountNumber + "'";
        
            var elem = " (function(doc, found) {" +
                     " window.addEventListener('DOMSubtreeModified', function() {" +
                     " var yourdiv = doc.querySelector('#btnPayNext');" +
                     " if (found && !yourdiv){" +
                     " found = false;}" +
                     " if (yourdiv) {" +
                     " document.getElementById('btnPayNext').style.display = 'none';" +
                     //" try{var xhttp = new XMLHttpRequest();"+
                     //" xhttp.onreadystatechange = function() {};"+
                     //" xhttp.open('GET', 'https://www.yomoneyservice.com/Mobile/invokeCsAction?message=ConfirmWebview-ChangeVMState-Confirmed&Id=" + uname + "', true); xhttp.send(); }catch(err){}" +
                     " }}, false);" +
                     " })(document, false);";
           
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await ConfirmWebview.EvaluateJavaScriptAsync($"" + js);
                            await ConfirmWebview.EvaluateJavaScriptAsync($"" + elem); 
                        });

                        if (viewModel.Error404 == false)
                        {
                            viewModel.IsNotBusy = true;
                            viewModel.IsConfirmed = true;
                            Task.Delay(2000).Wait();
                        }
                        else
                        {
                            viewModel.IsNotBusy = false;
                            viewModel.IsBusy  = false;
                        }
        }
    }


}