using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;

namespace YomoneyApp.Views.Fileuploads
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PictureFileUpload : ContentPage
    {
        private MediaFile _mediaFile;

        public PictureFileUpload()
        {
            InitializeComponent();
        }

        #region Pick Photo Region
        private async void btnPickPhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Photo Picked", "No Photo picked", "Ok");
            }

            _mediaFile = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFile == null)
                return;

            //LocalPathLabel.Text = _mediaFile.Path;

           // var file = await MediaPicker.PickPhotoAsync();

            //if (file == null)
            //    return;            

            FileImage.Source = ImageSource.FromStream(() =>
            {
                btnUploadImage.IsVisible = true;
                return _mediaFile.GetStream();
            });
        }
        #endregion

        private async void btnTakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No Camera Available", "Ok");
                return;
            }

            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "myImage.jpg"
            });

            FileImage.Source = ImageSource.FromStream(() =>
            {
                btnUploadImage.IsVisible = true;
                return _mediaFile.GetStream();
            });
        }
        private async void btnUploadImage_Clicked(object sender, EventArgs e)
        {
            FileUpload fileUpload = new FileUpload();
            AccessSettings accessSettings = new AccessSettings();

           // var stream = _mediaFile.GetStream();
            //var bytes = new byte[stream.Length];
            //await stream.ReadAsync(bytes, 0, (int)stream.Length);
            //string base64 = System.Convert.ToBase64String(bytes);

            //string strPath = _mediaFile.Path;

            //var fileName = Path.GetFileName(strPath); // filename

            //char[] delimite = new char[] { '.' };

            //string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

            //var type = parts[1];

            var file = await MediaPicker.PickPhotoAsync();

            if (file == null)
                return;

            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(await file.OpenReadAsync()), "file", file.FileName);

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://www.yomoneyservice.comUploadFile", content);

            StatusLabel.Text = response.StatusCode.ToString();

            //fileUpload.Name = fileName;

            //fileUpload.Type = type;

            //fileUpload.PhoneNumber = accessSettings.UserName;

            //fileUpload.Image = base64;

            //var imageBase = new ImageBase(base64);

            //try
            //{
            //    string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
            //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //    httpWebRequest.ContentType = "application/json";
            //    httpWebRequest.Method = "POST";
            //    httpWebRequest.Timeout = 120000;
            //    //httpWebRequest.CookieContainer = new CookieContainer();
            //    //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
            //    //cookie.Domain = "https://www.yomoneyservice.com";
            //    //httpWebRequest.CookieContainer.Add(cookie);

            //    var json = JsonConvert.SerializeObject(fileUpload);

            //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //    {
            //        streamWriter.Write(json);
            //        streamWriter.Flush();
            //        streamWriter.Close();

            //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //        {
            //            var result = streamReader.ReadToEnd();
            //            //yoappResponse = JsonConvert.DeserializeObject<YoAppResponse>(result);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            #region Commented Out Code
            //try
            //{

            //        var stream = _mediaFile.GetStream();
            //        var bytes = new byte [stream.Length];
            //        await stream.ReadAsync(bytes, 0, (int)stream.Length);
            //        string base64 = System.Convert.ToBase64String(bytes);

            //       var imageBase = new ImageBase(base64);
            //    var json = JsonConvert.SerializeObject(imageBase);
            //    var data = new StringContent(json, Encoding.UTF8, "application/json");

            //    // var content = new MultipartFormDataContent();

            //    // content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");

            //    var uploadUrl = "https://www.yomoneyservice.com/Mobile/FileUploader";

            //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
            //   (
            //       delegate { return true; }
            //   );

            // /*  System.Net.ServicePointManager.SecurityProtocol =
            //   SecurityProtocolType.Tls12 |
            //   SecurityProtocolType.Tls11 |
            //   SecurityProtocolType.Tls;*/

            //    var client = new HttpClient();

            //  //  string content = "id";

            //    var myContent = base64;

            //    //string paramlocal = string.Format(uploadUrl + "/Mobile/FileUploader/?{0}", content + "=This is a string");

            //    var httpResponseMessage = await client.PostAsync(uploadUrl, data);

            //  var response = httpResponseMessage.Content.ReadAsStringAsync();

            //   // string result = await client.GetStringAsync(paramlocal);

            //   //  if (result != "System.IO.MemoryStream")
            //   //  { 

            //   //  }

            //   //var httpResponseMessage = await httpClient.PostAsync(uploadUrl, content);               

            //    //var response = httpResponseMessage.Content.ReadAsStringAsync();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            #endregion

        }

        private void btnPickUpload_Clicked(object sender, EventArgs e)
        {

        }
    }
}