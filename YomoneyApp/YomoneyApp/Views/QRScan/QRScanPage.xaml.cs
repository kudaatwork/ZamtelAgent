using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Views.Services;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;


namespace YomoneyApp.Views.QRScan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScanPage : ContentPage
    {
        SpendViewModel viewModel;
        public QRScanPage()
        {
            InitializeComponent();
            ScannerView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.DATA_MATRIX,
                    BarcodeFormat.PDF_417,
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.MAXICODE,
                    BarcodeFormat.UPC_EAN_EXTENSION
                },
                TryHarder = true,
                AutoRotate = false,
                TryInverted = true,
                DelayBetweenContinuousScans = 2000,
            };

            ScannerView.OnScanResult += (result) =>
            {
                // var x = 3; // Breakpoint here, never hit
                if (ScannerView.IsScanning)                {
                    ScannerView.AutoFocus();
                }

                // Pop the page and show the result
               
                   Device.BeginInvokeOnMainThread(async () =>
                   {
                    // Stop analysis until we navigate away so we don't keep reading barcodes
                     ScannerView.IsAnalyzing = false;

                    // Show an alert
                   
                       string myinput = await InputBox(this.Navigation, "Payment", result.Text);
                       if (!string.IsNullOrEmpty(myinput))
                       {
                           char[] delimiter = new char[] { '\r', '\n' };
                           string[] part = result.Text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                           string accr = part[0];
                           MenuItem mn = new YomoneyApp.MenuItem();
                           mn.Description = result.Text;
                           mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(myinput), 2).ToString());
                           mn.Title = accr;
                           await Navigation.PushAsync(new PaymentPage(mn));
                       }
                       //viewModel.GetMasterPay(mn);
                });
            };

            ScannerOverlay.ShowFlashButton = ScannerView.HasTorch;
            ScannerOverlay.FlashButtonClicked += (se, ev) => ScannerView.ToggleTorch();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ScannerView.IsAnalyzing = true;
            ScannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ScannerView.IsScanning = false;
        }

        public static Task<string> InputBox(INavigation navigation, string Title, string message)
        {
            // wait in this proc, until user did his input 
            var tcs = new TaskCompletionSource<string>();
            var amount = "";
            char[] delimiter = new char[] { '\r','\n' };
            string[] part = message.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string accr = part[0];
            string ttl = " " ;
            string qsn = " ";
            if (part.Length == 3)
            { 
                ttl = part[0];
                char[] delimit = new char[] { ' ' };
                string[] partz = part[1].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                if (partz.Length == 2)
                {
                    amount = String.Format("{0:n}", Math.Round(decimal.Parse(partz[1]), 2).ToString()); 
                }
                else
                {
                    amount = String.Format("{0:n}", Math.Round(decimal.Parse(partz[0]), 2).ToString());
                }
            }
            else if (part.Length == 2)
            {
                qsn = "Enter Payment Amount";
            }
            else
            {
                ttl = "this code is not supported";
            }

            var lblTitle = new Label { Text = Title, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            var lblMessage = new Label { Text = ttl };
            var lblQuestion = new Label { Text = qsn, TextColor = Color.FromHex("#e2762b") };

            var txtInput = new Entry { Text = amount };
            if (part.Length == 3)
            {
                txtInput.IsEnabled = false;
            }
            var btnOk = new Button
            {
                Text = "Ok",
                WidthRequest = 100,
                BackgroundColor = Color.Green,
            };
            btnOk.Clicked += async (s, e) =>
            {
                // close page
                var result = txtInput.Text;
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await navigation.PopModalAsync();
                // pass result
                tcs.SetResult(result);
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                WidthRequest = 100,
                BackgroundColor = Color.FromHex("#e2762b")
            };
            btnCancel.Clicked += async (s, e) =>
            {
                // close page
                await App.Current.MainPage.Navigation.PopModalAsync();
                //await navigation.PopModalAsync();
                // pass empty result
                tcs.SetResult(null);
            };

            var slButtons = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnOk, btnCancel },
            };

            var layout = new StackLayout
            {
                Padding = new Thickness(0, 40, 0, 0),
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Vertical,
                Children = { lblTitle, lblMessage, lblQuestion, txtInput, slButtons },
            };

            // create and show page
            var page = new ContentPage();
            page.Content = layout;
            navigation.PushModalAsync(page);
            // open keyboard
            txtInput.Focus();

            // code is waiting her, until result is passed with tcs.SetResult() in btn-Clicked
            // then proc returns the result
            return tcs.Task;
        }

    }
}