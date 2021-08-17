using Newtonsoft.Json;
using PanCardView.Processors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Questions;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainSecurityQuestions : ContentPage
    {
        AccountViewModel viewModel;

        MenuItem mn = new MenuItem();

        public MainSecurityQuestions()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadQuestions();
        }

        public async void LoadQuestions()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://192.168.100.172:5000/Mobile/TransactionI";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);           
            content = await response.Content.ReadAsStringAsync();
            var quetionsList = JsonConvert.DeserializeObject<List<SecurityQuestions>>(content);
            //ListView1.ItemsSource = quetionsList;
        }

        private async void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SecurityQuestion());
        }
    }
}