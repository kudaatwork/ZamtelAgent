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

        public MainSecurityQuestions(string phone)
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
            viewModel.PhoneNumber = phone;
            LoadQuestions();
        }       

        public async void LoadQuestions()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://192.168.100.150:5000/Mobile/LoadQuestions";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var quetionsList = JsonConvert.DeserializeObject<List<SecurityQuestions>>(content);


            //ListView1.ItemsSource = quetionsList;
        }

        public List<SecurityQuestions> SecurityQuestions { get; set; }

        private async void btnSecurityQtnOption_Clicked(object sender, EventArgs e)
        {
           // await Navigation.PushAsync(new SecurityQuestion());
        }
    }
}