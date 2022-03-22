using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TakePhoto : ContentPage
    {
        private MediaFile _mediaFile;
        ServiceViewModel viewModel;
        MenuItem SelectedItem;

        public TakePhoto(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, mnu);
            SelectedItem = mnu;
            Title = mnu.Title;
        }

        private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            viewModel.IsBusy = true;

            #region File Upload via Base 64 String

            //AccessSettings acnt = new AccessSettings();
            //string pass = acnt.Password;
            //string uname = acnt.UserName;

            //bool saved = false;

            //FileUpload fileUpload = new FileUpload();

            //var stream = _mediaFile.GetStream();
            //var bytes = new byte[stream.Length];
            //await stream.ReadAsync(bytes, 0, (int)stream.Length);
            //string base64 = System.Convert.ToBase64String(bytes);

            //string strPath = _mediaFile.Path;

            //var fileName = Path.GetFileName(strPath); // filename

            //char[] delimite = new char[] { '.' };

            //string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

            //var type = parts[1];

            //var imageStrings = Split(base64, 0);

            //fileUpload.Name = fileName;
            //fileUpload.Type = type;
            //fileUpload.PhoneNumber = uname;
            //fileUpload.Image = imageStrings[0];
            //fileUpload.Image1 = imageStrings[1];
            //fileUpload.Image2 = imageStrings[2];
            //fileUpload.Image3 = imageStrings[3];
            //fileUpload.Image4 = imageStrings[4];
            //fileUpload.Image5 = imageStrings[5];
            //fileUpload.Image6 = imageStrings[6];
            //fileUpload.Image7 = imageStrings[7];
            //fileUpload.Image8 = imageStrings[8];
            //fileUpload.Image9 = imageStrings[9];
            //fileUpload.Purpose = "Profile Picture";
            //fileUpload.ServiceId = 0;
            //fileUpload.ActionId = 0;
            //fileUpload.SupplierId = "";

            //try
            //{
            //    string url = String.Format("http://102.130.120.163:8058/api/fileupload");
            //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //    httpWebRequest.ContentType = "application/json";
            //    httpWebRequest.Method = "POST";
            //    httpWebRequest.Timeout = 120000;
            //    //httpWebRequest.CookieContainer = new CookieContainer();
            //    //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
            //    //cookie.Domain = "http://192.168.100.150:5000";
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

            //            var stringResult = JsonConvert.DeserializeObject<string>(result);

            //            if (stringResult == "Success")
            //            {
            //                viewModel.IsBusy = false;
            //                saved = true;
            //            }
            //            else
            //            {
            //                viewModel.IsBusy = false;
            //                saved = false;
            //            }
            //            //FileImage.Source.ClearValue();

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    saved = false;
            //}

            //if (saved)
            //{
            //    await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
            //    //await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
            //    await Navigation.PushAsync(new HomePage());
            //}
            //else
            //{
            //    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
            //    viewModel.IsBusy = false;
            //}

            #endregion

            #region File Upload via Multipart           

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://102.130.120.163:8058/");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new MultipartFormDataContent();

                    content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");

                    var uploadServiceBaseAddress = "api/ProfileImages/Upload";

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    httpClient.Timeout = TimeSpan.FromMinutes(3);

                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                    var response = httpResponseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<string>(response.Result);

                    //var result = response.Result;

                    if (response.Result.ToUpper() == "FAILED")
                    {
                        await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                        viewModel.IsBusy = false;
                    }
                    else
                    {
                        AccessSettings acnt = new AccessSettings();
                        string pass = acnt.Password;
                        string uname = acnt.UserName;

                        FileUpload fileUpload = new FileUpload();

                        string strPath = _mediaFile.Path;

                        var fileName = Path.GetFileName(strPath); // filename

                        char[] delimite = new char[] { '.' };

                        string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                        var type = parts[1];

                        var finalFileName = fileName.Replace(" ", "_");

                        //fileName.Replace(" ", "_");

                        fileUpload.Name = finalFileName;
                        fileUpload.Type = type;
                        fileUpload.PhoneNumber = uname;
                        fileUpload.Image = "http://102.130.120.163:8058" + result;
                        fileUpload.Purpose = HomeViewModel.fileUpload.Purpose;
                        fileUpload.ServiceId = HomeViewModel.fileUpload.ServiceId;
                        fileUpload.ActionId = HomeViewModel.fileUpload.ActionId;
                        fileUpload.SupplierId = HomeViewModel.fileUpload.SupplierId;
                        fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                        fileUpload.FieldId = HomeViewModel.fileUpload.FieldId;

                        string url = String.Format("http://192.168.100.150:5000/Mobile/FileUploader");
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Timeout = 120000;
                        httpWebRequest.CookieContainer = new CookieContainer();
                        Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                        cookie.Domain = "www.yomoneyservice.com";
                        httpWebRequest.CookieContainer.Add(cookie);

                        var json = JsonConvert.SerializeObject(fileUpload);

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();

                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var serverresult = streamReader.ReadToEnd();

                                if (serverresult.Contains("/Mobile"))
                                {
                                    await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
                                    viewModel.IsBusy = false;
                                    FileImage.Source = null;
                                    await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "Take Photo", false, null));
                                }
                                else
                                {
                                    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                                    viewModel.IsBusy = false;
                                }

                                //var stringResult = JsonConvert.DeserializeObject<string>(result);

                                //if (stringResult == "Success")
                                //{
                                //    await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
                                //    viewModel.IsBusy = false;

                                //    await Navigation.PushAsync(new HomePage());
                                //}
                                //else
                                //{
                                //    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                                //    viewModel.IsBusy = false;
                                //}
                                //FileImage.Source.ClearValue();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);

                    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                    viewModel.IsBusy = false;
                }
            }

            #endregion            
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable ||
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

            FileStatus.Text = Path.GetFileName(_mediaFile.Path);
            FileStatus.TextColor = Color.FromHex("#22b24c");

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });
        }
    }
}