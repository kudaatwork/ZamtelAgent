using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;
using System.Net.Http;
using YomoneyApp.Services;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileUploadPage : ContentPage
    {
        ServiceViewModel viewModel;
        MenuItem SelectedItem;
        private MediaFile _mediaFile;
        public FileUploadPage(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this,mnu);
            SelectedItem = mnu;
            Title = mnu.Title;
                
        }

        private async void PickPhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
    
            if(!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No PickedPhoto", "No Photos in Galary", "Ok");
                return;
            }

            _mediaFile = await CrossMedia.Current.PickPhotoAsync();
            if (_mediaFile == null) return;

            FileImage.Source = ImageSource.FromStream(() =>
           {
               return _mediaFile.GetStream();
           });
        }  

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
                if(! CrossMedia.Current.IsCameraAvailable || 
                !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }
            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "MyImage.jpg"
            });
            if (_mediaFile == null) return;

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();  
            });
        }

      /*  private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;
            ByteArrayContent byteArrayContent = new ByteArrayContent(File.ReadAllBytes(_mediaFile.Path));

            Uri webService = new Uri("http://192.168.100.172:5001/api/vend/Upload");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
            requestMessage.Headers.ExpectContinue = false;

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
            //ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
            byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
            Guid uniqueId = Guid.NewGuid();
            multiPartContent.Add(byteArrayContent, "file", "image" + uniqueId + ".jpg");
            requestMessage.Content = multiPartContent;
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage httpRequest = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
                var responseContent = await httpRequest.Content.ReadAsStringAsync();
               // ProgressBarWidth += Percentage;
            }
            catch (Exception u)
            {
                if (u.Message == "Object reference not set to an instance of an object.")
                {
                    await DisplayAlert("Error", "Please select an image or take a photo.", "OK");
                }
                else
                {
                    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                }
            }
        }*/
        
        private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            AccessSettings acnt = new AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;
            try
            {
                //HttpContent fileStreamContent = new StreamContent(_mediaFile.GetStream());
                /*fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "test" };
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                //ByteArrayContent baContent = new ByteArrayContent($"\"{_mediaFile.Path}\"");
                var fileContent = new ByteArrayContent(File.ReadAllBytes(_mediaFile.Path));
               
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("image")
                {
                    FileName = uname
                };

                formData.Add(fileContent);*/
                var formData = new MultipartFormDataContent();
                var httpClient = new HttpClient();
                //httpClient.MaxResponseContentBufferSize = 82113340;
                
                formData.Add(new StreamContent(_mediaFile.GetStream()),
           "\"file\"",
           $"\"{_mediaFile.Path}\"");
                var uploadBaseAddress = "http://192.168.100.172:5001/api/Vend/Upload";

               // content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
           
                var httpResponseMessage = await httpClient.PostAsync(uploadBaseAddress, formData);
                var res = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await DisplayAlert("Error", httpResponseMessage.StatusCode.ToString(), "OK");
                }
                else
                {
                    await DisplayAlert("Success", "Image uploaded successfuly.", "OK");
                }
            }
            catch(Exception u) 
            {
                if (u.Message == "Object reference not set to an instance of an object.")
                {
                    await DisplayAlert("Error", "Please select an image or take a photo.", "OK");
                }
                else
                {
                    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                }
            }
        }
        
    }
}