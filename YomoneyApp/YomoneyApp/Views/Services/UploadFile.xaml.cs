using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;
using YomoneyApp.Interfaces;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadFile : ContentPage
    {
        private MediaFile _mediaFile;
        ServiceViewModel viewModel;
        MenuItem SelectedItem;
        public FileResult fileResult;

        public UploadFile(MenuItem mnu)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, mnu);
            SelectedItem = mnu;
            Title = mnu.Title;
        }

        private async void PickPhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
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

        private async void PickFile_Clicked(object sender, EventArgs e)
        {
            #region File Picker

            var customFileType =
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.text" } }, // or general UTType values
                { DevicePlatform.Android, new[] { "text/plain", "text/csv", "application/msword", "application/vnd.openxmlformats",
                "application/pdf", "application/vnd.ms-powerpoint", "application/vnd.rar", "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/zip"} },
            });

            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = customFileType,
            };

            fileResult = await FilePicker.PickAsync(options);

            if (fileResult != null)
            {
                char[] delimite = new char[] { '.' };

                string[] parts = fileResult.FileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                if (parts[1] == "png" || parts[1] == "jpg" || parts[1] == "jpeg")
                {
                    SavePhoto.IsEnabled = true;

                    FileStream fileStream = new FileStream(fileResult.FullPath, FileMode.Open, FileAccess.Read);

                    FileStatus.Text = fileResult.FileName;
                    FileStatus.TextColor = Xamarin.Forms.Color.FromHex("#22b24c");

                    FileImage.Source = ImageSource.FromStream(() =>
                    {
                        return fileStream;
                    });
                }
                else
                {
                    SavePhoto.IsEnabled = true;

                    FileStatus.Text = fileResult.FileName;
                    FileStatus.TextColor = Xamarin.Forms.Color.FromHex("#22b24c");

                    FileImage.Source = "attachment1.png";
                }
            }

            #endregion
        }

        private async void PickImage_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }

            _mediaFile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 65,
                CustomPhotoSize = 50
            });

            if (_mediaFile == null)
                return;

            SavePhoto.IsEnabled = true;

            FileStatus.Text = Path.GetFileName(_mediaFile.Path);
            FileStatus.TextColor = Xamarin.Forms.Color.FromHex("#22b24c");

            FileImage.Source = ImageSource.FromStream(() =>
            {
                var stream = _mediaFile.GetStream();               
                return stream;
            });
        }

        private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            viewModel.IsBusy = true;

            #region File Upload via Multipart           

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://102.130.120.163:8058/");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //char[] delimiter = new char[] { '.' };

                    //string[] strParts = fileResult.FileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    string result = string.Empty;

                    if (fileResult == null) // It's an image
                    {
                        // var imagePath = CompressImage(_mediaFile.Path, Path.GetFileName(_mediaFile.Path), 40);
                        //ImageCompression imageCompression = new ImageCompression();

                        //FileStream fileStream = new FileStream(_mediaFile.GetStream(), FileMode.Open, FileAccess.Read);

                        //var bytes = new byte[fileStream.Length];

                        //var compressedImage = DependencyService.Get<IImageCompressionService>().CompressImage(bytes, fileResult.FullPath, 40);                       

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
                            var content = new MultipartFormDataContent();

                            content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");

                            var uploadServiceBaseAddress = "api/ProfileImages/Upload";

                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            httpClient.Timeout = TimeSpan.FromMinutes(3);

                            var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                            var response = httpResponseMessage.Content.ReadAsStringAsync();

                            result = JsonConvert.DeserializeObject<string>(response.Result);

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

                                        if (serverresult.Contains("/Mobile/"))
                                        {
                                            await DisplayAlert("File Upload", "File uploaded and saved successfully", "OK");
                                            viewModel.IsBusy = false;
                                            FileImage.Source = null;
                                            FileStatus.Text = null;
                                            SavePhoto.IsEnabled = false;
                                            _mediaFile.Dispose();
                                            await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com" + serverresult, "File Upload", false, null));

                                            //Device.BeginInvokeOnMainThread(async () =>
                                            //{
                                            //    await App.Current.MainPage.Navigation.PushAsync(new HomePage());
                                            //});
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
                    else
                    {
                        var content = new MultipartFormDataContent();

                        FileStream fileStream = new FileStream(fileResult.FullPath, FileMode.Open, FileAccess.Read);
                                              
                        string filePath = fileResult.FullPath;

                        long fileSizeibBytes = GetFileSize(filePath);

                        if (fileSizeibBytes > 2097152)
                        {
                            await DisplayAlert("Error!", "Your image is too large, it has to be less than 2 MB", "OK");
                            viewModel.IsBusy = false;
                            return;
                        }
                        else
                        {
                            content.Add(new StreamContent(fileStream), "\"file\"", $"\"{fileResult.FullPath}\"");

                            var uploadServiceBaseAddress = "api/ServerFiles/Upload";

                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                            httpClient.Timeout = TimeSpan.FromMinutes(3);

                            var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                            var response = httpResponseMessage.Content.ReadAsStringAsync();

                            result = JsonConvert.DeserializeObject<string>(response.Result);

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

                                FileUpload fileUpload = new FileUpload();

                                var fileName = Path.GetFileName(fileResult.FileName); // filename

                                char[] delimite = new char[] { '.' };

                                string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                                var type = parts[1];

                                var finalFileName = fileName.Replace(" ", "_");

                                //fileName.Replace(" ", "_");

                                fileUpload.Name = finalFileName;
                                fileUpload.Type = type;
                                fileUpload.PhoneNumber = uname;
                                fileUpload.Image = "http://102.130.120.163:8058" + result;
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

                                        if (serverresult.Contains("/Mobile/"))
                                        {
                                            await DisplayAlert("File Upload", "File uploaded and saved successfully", "OK");
                                            viewModel.IsBusy = false;
                                            FileImage.Source = null;
                                            FileStatus.Text = null;
                                            SavePhoto.IsEnabled = false;
                                            _mediaFile.Dispose();
                                            await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com" + serverresult, "File Upload", false, null));

                                            //Device.BeginInvokeOnMainThread(async () =>
                                            //{
                                            //    await App.Current.MainPage.Navigation.PushAsync(new HomePage());
                                            //});
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

                    //var result = response.Result;                    
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
