﻿using System;
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
using Newtonsoft.Json;
using YomoneyApp.Models.Image;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileUploadPage : ContentPage
    {
        ServiceViewModel viewModel;
        MenuItem SelectedItem;
        private MediaFile _mediaFile;
        string HostDomain = "https://www.yomoneyservice.com";

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
                await DisplayAlert("No PickedPhoto", "No Photos in Gallery", "Ok");
                return;
            }

            _mediaFile = await CrossMedia.Current.PickPhotoAsync();
            if (_mediaFile == null) return;

            SavePhoto.IsEnabled = true;
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
            SavePhoto.IsEnabled = true;
            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();  
            });
        }

        private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            viewModel.IsBusy = true;

            AccessSettings acnt = new AccessSettings();
            string pass = acnt.Password;
            string uname = acnt.UserName;

            bool saved = false;

            FileUpload fileUpload = new FileUpload();

            var stream = _mediaFile.GetStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            string base64 = System.Convert.ToBase64String(bytes);

            string strPath = _mediaFile.Path;

            var fileName = Path.GetFileName(strPath); // filename

            char[] delimite = new char[] { '.' };

            string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

            var type = parts[1];

            fileUpload.Name = fileName;
            fileUpload.Type = type;
            fileUpload.PhoneNumber = uname;
            fileUpload.Image = base64;
            fileUpload.Purpose = "Profile Picture";
            fileUpload.ServiceId = 0;
            fileUpload.ActionId = 0;
            fileUpload.SupplierId = "";

           try
            {
                string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 120000;
                //httpWebRequest.CookieContainer = new CookieContainer();
                //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                //cookie.Domain = "https://www.yomoneyservice.com";
                //httpWebRequest.CookieContainer.Add(cookie);

                var json = JsonConvert.SerializeObject(fileUpload);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();

                        var stringResult = JsonConvert.DeserializeObject<string>(result);

                        if (stringResult == "Success")
                        {
                            viewModel.IsBusy = false;
                            saved = true;
                        }
                        else
                        {
                            viewModel.IsBusy = false;
                            saved = false;
                        }                        
                        //FileImage.Source.ClearValue();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                saved = false;
            }

            if (saved)
            {    
                
                await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
                //await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
                await Navigation.PushAsync(new HomePage());
            }
            else
            {
                await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
            }

           // try
           // {
           //     //HttpContent fileStreamContent = new StreamContent(_mediaFile.GetStream());
           //     /*fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "test" };
           //     fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
           //     //ByteArrayContent baContent = new ByteArrayContent($"\"{_mediaFile.Path}\"");
           //     var fileContent = new ByteArrayContent(File.ReadAllBytes(_mediaFile.Path));
               
           //     fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("image")
           //     {
           //         FileName = uname
           //     };

           //     formData.Add(fileContent);*/
           //     var formData = new MultipartFormDataContent();
           //     var httpClient = new HttpClient();
           //     //httpClient.MaxResponseContentBufferSize = 82113340;
                
           //     formData.Add(new StreamContent(_mediaFile.GetStream()),
           //"\"file\"",
           //$"\"{_mediaFile.Path}\"");
           //     var uploadBaseAddress = "https://www.yomoneyservice.com/api/Vend/Upload";

           //    // content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
           //     //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
           
           //     var httpResponseMessage = await httpClient.PostAsync(uploadBaseAddress, formData);
           //     var res = await httpResponseMessage.Content.ReadAsStringAsync();
           //     if (httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
           //     {
           //         await DisplayAlert("Error", httpResponseMessage.StatusCode.ToString(), "OK");
           //     }
           //     else
           //     {
           //         await DisplayAlert("Success", "Image uploaded successfuly.", "OK");
           //     }
           // }
           // catch(Exception u) 
           // {
           //     if (u.Message == "Object reference not set to an instance of an object.")
           //     {
           //         await DisplayAlert("Error", "Please select an image or take a photo.", "OK");
           //     }
           //     else
           //     {
           //         await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
           //     }
           // }
        }
        
    }
}