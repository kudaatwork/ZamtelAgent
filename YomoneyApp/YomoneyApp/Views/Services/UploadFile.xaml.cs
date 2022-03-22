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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;

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
                { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                { DevicePlatform.Android, new[] { "*/*" } },
            });

            var options = new PickOptions
            {
                PickerTitle = "Please select a file",
                FileTypes = customFileType,
            };

            fileResult = await FilePicker.PickAsync(options);

            char[] delimite = new char[] { '.' };

            string[] parts = fileResult.FileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

            if (parts[1] == "png" || parts[1] == "jpg" || parts[1] == "jpeg")
            {
                SavePhoto.IsEnabled = true;

                FileStream fileStream = new FileStream(fileResult.FullPath, FileMode.Open, FileAccess.Read);

                FileStatus.Text = fileResult.FileName;
                FileStatus.TextColor = Color.FromHex("#22b24c");

                FileImage.Source = ImageSource.FromStream(() =>
                {
                    return fileStream;
                });
            }
            else
            {
                SavePhoto.IsEnabled = true;

                FileStatus.Text = fileResult.FileName;
                FileStatus.TextColor = Color.FromHex("#22b24c");

                FileImage.Source = "attachment1.png";
            }

            #endregion
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

                    char[] delimiter = new char[] { '.' };

                    string[] strParts = fileResult.FileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    string result = string.Empty;

                    if (strParts[1] == "png" || strParts[1] == "jpg" || strParts[1] == "jpeg")
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

                                    if (serverresult.Contains("/Mobile/"))
                                    {
                                        await DisplayAlert("File Upload", "File uploaded and saved successfully", "OK");
                                        viewModel.IsBusy = false;
                                        FileImage.Source = null;
                                        await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "File Upload", false, null));

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
                    else
                    {
                        var content = new MultipartFormDataContent();

                        FileStream fileStream = new FileStream(fileResult.FullPath, FileMode.Open, FileAccess.Read);

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

                                    if (serverresult.Contains("/Mobile/"))
                                    {
                                        await DisplayAlert("File Upload", "File uploaded and saved successfully", "OK");
                                        viewModel.IsBusy = false;
                                        FileImage.Source = null;
                                        await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "File Upload", false, null));

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

        #region Upload via Google Drive

        #region 1st Request

        //UserCredential credential;

        //using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        //{
        //    // The file token.json stores the user's access and refresh tokens, and is created
        //    // automatically when the authorization flow completes for the first time.
        //    string credPath = "token.json";

        //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        GoogleClientSecrets.Load(stream).Secrets,
        //        Scopes,
        //        "user",
        //        CancellationToken.None,
        //        new FileDataStore(credPath, true)).Result;

        //    Console.WriteLine("Credential file saved to: " + credPath);
        //}

        //// Create Drive API service.
        //var service = new DriveService(new BaseClientService.Initializer()
        //{
        //    HttpClientInitializer = credential,
        //    ApplicationName = ApplicationName,
        //});

        //// Define parameters of request.
        //FilesResource.ListRequest listRequest = service.Files.List();

        //listRequest.PageSize = 10;
        //listRequest.Fields = "nextPageToken, files(id, name)";

        //// List files.
        //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        //Console.WriteLine("Files:");

        //if (files != null && files.Count > 0)
        //{
        //    foreach (var file in files)
        //    {
        //        Console.WriteLine("{0} ({1})", file.Name, file.Id);
        //    }
        //}
        //else
        //{
        //    Console.WriteLine("No files found.");
        //}

        //Console.Read();

        #endregion

        #region 2nd Request

        //UserCredential credential;

        //using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        //{
        //    // The file token.json stores the user's access and refresh tokens, and is created
        //    // automatically when the authorization flow completes for the first time.
        //    string credPath = "token.json";

        //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        GoogleClientSecrets.Load(stream).Secrets,
        //        Scopes,
        //        "user",
        //        CancellationToken.None,
        //        new FileDataStore(credPath, true)).Result;

        //    Console.WriteLine("Credential file saved to: " + credPath);
        //}

        //// Create Drive API service.
        //var service = new DriveService(new BaseClientService.Initializer()
        //{
        //    HttpClientInitializer = credential,
        //    ApplicationName = ApplicationName,
        //});

        //// Define parameters of request.
        //FilesResource.ListRequest listRequest = service.Files.List();

        //listRequest.PageSize = 10;
        //listRequest.Fields = "nextPageToken, files(id, name)";

        //// List files.
        //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        //Console.WriteLine("Files:");

        //if (files != null && files.Count > 0)
        //{
        //    foreach (var file in files)
        //    {
        //        Console.WriteLine("{0} ({1})", file.Name, file.Id);
        //    }
        //}
        //else
        //{
        //    Console.WriteLine("No files found.");
        //}

        //Console.Read();

        #endregion

        #region 3rd Request

        //bool saved = false;

        //FileUpload fileUpload = new FileUpload();

        //var stream = _mediaFile.GetStream();
        //var bytes = new byte[stream.Length];
        //await stream.ReadAsync(bytes, 0, (int)stream.Length);
        //string base64 = System.Convert.ToBase64String(bytes);

        //string strPath = _mediaFile.Path;

        //var fileNameFromFile = Path.GetFileName(strPath); // filename

        //char[] delimite = new char[] { '.' };

        //string[] parts = fileNameFromFile.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

        //var type = parts[1];


        //string fileName = string.Empty;
        //string fileMime = string.Empty;
        //string folder = string.Empty;
        //string fileDescription = string.Empty;

        //DriveService service = GetService();


        //fileName = fileNameFromFile;
        //fileMime = "image/jpeg";
        //folder = "uploads";

        //using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        //{
        //    var driveFile = new Google.Apis.Drive.v3.Data.File();
        //    driveFile.Name = fileName;
        //    driveFile.Description = fileDescription;
        //    driveFile.MimeType = fileMime;
        //    driveFile.Parents = new string[] { folder };

        //    var request = service.Files.Create(driveFile, file, fileMime);
        //    request.Fields = "*";

        //    var response = request.Upload();

        //    if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
        //        throw response.Exception;

        //    var responseId = request.ResponseBody.Id;

        //    saved = true;

        //    if (saved)
        //    {
        //        await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
        //        //await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
        //        await Navigation.PushAsync(new HomePage());
        //    }
        //    else
        //    {
        //        await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
        //        viewModel.IsBusy = false;
        //    }
        //}

        #endregion

        #endregion

        #region Commented Out Code

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
        //     var uploadBaseAddress = "http://192.168.100.150:5000/api/Vend/Upload";

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




        //private static DriveService GetService()
        //{
        //    var tokenResponse = new TokenResponse
        //    {
        //        AccessToken = AppConstants.AccessToken,
        //        RefreshToken = AppConstants.RefreshToken,
        //    };

        //    var applicationName = AppConstants.ApplicationName; // Use the name of the project in Google Cloud
        //    var username = AppConstants.Username; // Use your email

        //    var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = AppConstants.ClientId,
        //            ClientSecret = AppConstants.ClientSecret
        //        },

        //        Scopes = new[] { Scope.Drive },
        //        DataStore = new FileDataStore(applicationName)
        //    });

        //    var credential = new UserCredential(apiCodeFlow, username, tokenResponse);

        //    var service = new DriveService(new BaseClientService.Initializer
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = applicationName
        //    });

        //    return service;
        //}

        //public string CreateFolder(string parent, string folderName)
        //{
        //    var service = GetService();

        //    var driveFolder = new Google.Apis.Drive.v3.Data.File();

        //    driveFolder.Name = folderName;
        //    driveFolder.MimeType = "application/vnd.google-apps.folder";
        //    driveFolder.Parents = new string[] { parent };

        //    var command = service.Files.Create(driveFolder);
        //    var file = command.Execute();

        //    return file.Id;
        //}

        #endregion

        
    }
}
