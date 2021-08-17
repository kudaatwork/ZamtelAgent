using MvvmHelpers;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;
using YomoneyApp.Views.Spani;
using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YomoneyApp
{
    public class QRViewModel : ViewModelBase
    {
        string HostDomain = "http://192.168.100.172:5000";
        bool showAlert = false;
        public ObservableRangeCollection<MenuItem> myItemsSource { get; set; }
        private ZXing.Result result;
        public ZXing.Result Result
        {
            get { return this.result; }
            set
            {
                if (!string.Equals(this.result, value))
                {
                    this.result = value;
                    this.OnPropertyChanged(nameof(Result));
                }
            }
        }
        public QRViewModel(Page page) : base(page)
        {
            Title = "QRPay";
            Position = 0;
            myItemsSource = new ObservableRangeCollection<MenuItem>();
            //TemplateSelector = new MyTemplateSelector(); //new DataTemplate (typeof(MyView));
        }
       
        public int Position { get; set; }
        
    }
}

