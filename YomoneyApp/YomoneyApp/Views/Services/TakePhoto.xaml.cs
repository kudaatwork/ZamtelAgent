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
using Xamarin.Forms.PlatformConfiguration;
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

        string HostDomain = "https://www.yomoneyservice.com";
        string webviewLink = "/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&serviceId=" + HomeViewModel.fileUpload.ServiceId + "&ActionId=" + HomeViewModel.fileUpload.ActionId +
            "&FormNumber=" + HomeViewModel.fileUpload.FormId + "&Customer=" + HomeViewModel.fileUpload.PhoneNumber + "&CallType=FirstTime";
        string title = "";

        public TakePhoto(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, mnu);
            SelectedItem = mnu;
            Title = mnu.Title;
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            //await CrossMedia.Current.Initialize();
            //if (!CrossMedia.Current.IsCameraAvailable ||
            //!CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    await DisplayAlert("No Camera", "No camera available", "Ok");
            //}

            //_mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            //{
            //    Directory = "Sample",
            //    Name = "MyImage.jpg",
            //    CompressionQuality = 65,                
            //    RotateImage = false
            //}) ;

            //if (_mediaFile == null) return;

            //SavePhoto.IsEnabled = true;

            //FileStatus.Text = Path.GetFileName(_mediaFile.Path);
            //FileStatus.TextColor = Color.FromHex("#22b24c");

            //FileImage.Source = ImageSource.FromStream(() =>
            //{
            //    return _mediaFile.GetStream();
            //});

            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable ||
            !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }

            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "MyImage.jpg",
                CompressionQuality = 60
            });

            if (_mediaFile == null) return;

            SavePhoto.IsEnabled = true;

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    Navigation.PopAsync();

            //    await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + webviewLink, title, false, null, false));
            //});            
        }

        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform.Equals(Device.UWP))
            {
                //OnClosePageRequested();
                return true;
            }
            else
            {
                base.OnBackButtonPressed();
                return false;
            }
        }

        //async void OnClosePageRequested()
        //{
        //    var tdvm = (TaskDetailsViewModel)BindingContext;
        //    if (tdvm.CanSaveTask())
        //    {
        //        var result = await DisplayAlert("Wait", "You have unsaved changes! Are you sure you want to go back?", "Discard changes", "Cancel");

        //        if (result)
        //        {
        //            tdvm.DiscardChanges();
        //            await Navigation.PopAsync(true);
        //        }
        //    }
        //    else
        //    {
        //        await Navigation.PopAsync(true);
        //    }
        //}

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
            //    string url = String.Format("https://www.yomoneyservice.comapi/fileupload");
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
            //    //await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
            //    await Navigation.PushAsync(new HomePage());
            //}
            //else
            //{
            //    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
            //    viewModel.IsBusy = false;
            //}

            #endregion

            #region File Upload via Multipart           

            try
            {
                var baseAddress = new Uri("https://www.yomoneyservice.com");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new MultipartFormDataContent();

                    content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");

                    var uploadServiceBaseAddress = "api/ProfileImages/Upload";

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    client.Timeout = TimeSpan.FromMinutes(3);

                    cookieContainer.Add(baseAddress, new Cookie("AspxAutoDetectCookieSupport", "1", "/", "www.yomoneyservice.com"));

                    string filePath = _mediaFile.Path;

                    long fileSizeibBytes = GetFileSize(filePath);

                    if (fileSizeibBytes > 2097152)
                    {
                        await DisplayAlert("Error!", "Your image is too large, it has to be less than 2 MB", "OK");
                        viewModel.IsBusy = false;
                        return;
                    }
                    else
                    {
                        var httpResponseMessage = await client.PostAsync(uploadServiceBaseAddress, content);

                        var response = httpResponseMessage.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<string>(response.Result);

                        if (result.ToUpper() == "FAILED")
                        {
                            await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                            viewModel.IsBusy = false;
                        }
                        else
                        {
                            AccessSettings acnt = new AccessSettings();
                            string pass = acnt.Password;
                            string uname = acnt.UserName;

                            if (string.IsNullOrEmpty(pass))
                            {
                                pass = AccountViewModel.Password;
                            }

                            if (string.IsNullOrEmpty(uname))
                            {
                                uname = AccountViewModel.ActualPhoneNumber;
                            }

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
                            fileUpload.Image = "https://www.yomoneyservice.com" + result;
                            fileUpload.Purpose = "FORMS";
                            fileUpload.ServiceId = HomeViewModel.fileUpload.ServiceId;
                            fileUpload.ActionId = HomeViewModel.fileUpload.ActionId;
                            fileUpload.SupplierId = HomeViewModel.fileUpload.SupplierId;
                            fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                            fileUpload.FieldId = HomeViewModel.fileUpload.FieldId;

                            if (!string.IsNullOrEmpty(HomeViewModel.fileUpload.RecordId))
                            {
                                fileUpload.RecordId = HomeViewModel.fileUpload.RecordId;
                            }
                            else
                            {
                                fileUpload.RecordId = "0";
                            }

                            string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Timeout = 120000;
                            httpWebRequest.CookieContainer = new CookieContainer();
                            Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                            cookie.Domain = "192.168.100.150";
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
                                        //FileStatus.Text = null;
                                        SavePhoto.IsEnabled = false;
                                        //_mediaFile.Dispose();

                                        //Device.BeginInvokeOnMainThread(async () =>
                                        //{
                                        //    await Navigation.PopAsync();
                                        //    //await Navigation.PopToRootAsync();
                                        //});

                                        Navigation.PopAsync();

                                        await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com" + serverresult, "Take Photo", false, null));

                                        //await Navigation.PopAsync();
                                        //await Navigation.PopToRootAsync();

                                        //this.Navigation.RemovePage(this.Navigation.NavigationStack[this.]);

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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);

                await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                viewModel.IsBusy = false;
            }

            #endregion
        }

        static long GetFileSize(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return new FileInfo(FilePath).Length;
            }

            return 0;
        }
    }
}