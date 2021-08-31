using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SignaturePad.Forms;
using Plugin.Media.Abstractions;
using YomoneyApp.Services;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignaturePage : ContentPage
    {
       
        MenuItem SelectedItem;
        private Point[] points;
        private MediaFile _mediaFile;

        public SignaturePage(MenuItem mnu)
        {
            InitializeComponent();
            //BindingContext = new SignatureViewModel(SignaturePadView.GetImageStreamAsync);
            UpdateControls();
            SelectedItem = mnu;
        }

        private void UpdateControls()
        {
           // btnSave.IsEnabled = !signatureView.IsBlank;
            btnSaveImage.IsEnabled = !signatureView.IsBlank;
            //btnLoad.IsEnabled = points != null;
        }

        private void SaveVectorClicked(object sender, EventArgs e)
        {
            points = signatureView.Points.ToArray();
            UpdateControls();

            DisplayAlert("Signature Pad", "Vector signature saved to memory.", "OK");
        }

        private void LoadVectorClicked(object sender, EventArgs e)
        {
            signatureView.Points = points;
        }

        private async void SaveImageClicked(object sender, EventArgs e)
        {
            bool saved;
            using (var bitmap = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png))
            {
                try
                {
                    saved = true;//await SaveSignature(bitmap, "signature.png");
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(bitmap));

                    var httpClient = new HttpClient();
                    AccessSettings acnt = new AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;
                    var uploadBaseAddress = "http://192.168.100.172:5001/Mobile/FileUpload?user=" + uname + ":" + pass + "&upType=Signature" ;
                    var httpResponseMessage = await httpClient.PostAsync(uploadBaseAddress, content);
                   
                    if (saved)
                        await DisplayAlert("Signature Pad", "Raster signature saved to the photo library.", "OK");
                    else
                        await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                }
                catch
                {

                }
            }

        }

        private void SignatureChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }
    }
}  