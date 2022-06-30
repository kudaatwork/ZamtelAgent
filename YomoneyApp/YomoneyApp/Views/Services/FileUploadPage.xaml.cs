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
using Newtonsoft.Json;
using YomoneyApp.Models.Image;
using YomoneyApp.Views.Webview;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using static Google.Apis.Drive.v3.DriveService;
using YomoneyApp.Constants;
using System.Text.RegularExpressions;

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
            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });
        }


        static List<string> Split(string str, int chunkSize)
        {
            var outputList = new List<string>();

            if (!string.IsNullOrEmpty(str))
            {
                var extraInput = str.Length % 10;

                chunkSize = (str.Length - extraInput) / 10;

                for (int i = 1; i <= 10; i++)
                {
                    if (i != 10)
                    {
                        string currentString = str.Substring((i - 1) * 10, chunkSize);

                        outputList.Add(currentString);
                    }
                    else
                    {
                        string currentString = str.Substring((i - 1) * 10, chunkSize + extraInput);

                        outputList.Add(currentString);
                    }
                }

                // var output = Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));

                //if (input != 0)
                //{
                //    outputList = output.ToList();

                //}

                return outputList;
            }
            else
            {
                return null;
            }
        }

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json  
        //static string[] Scopes = { DriveService.Scope.DriveReadonly };
        //static string ApplicationName = "Drive API .NET Quickstart";

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

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("https://www.yomoneyservice.com");
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
                        fileUpload.Purpose = "Profile Picture";
                        fileUpload.ServiceId = 0;
                        fileUpload.ActionId = 0;
                        fileUpload.SupplierId = "";

                        string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Timeout = 120000;
                        httpWebRequest.CookieContainer = new CookieContainer();
                        Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                        cookie.Domain = "www.yomoneyservice.com:5001";
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

                                if (serverresult.Contains("/Mobile/JobProfile"))
                                {
                                    await DisplayAlert("File Upload", "Image uploaded and saved successfully", "OK");
                                    viewModel.IsBusy = false;
                                    FileImage.Source = null;
                                    await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com" + serverresult, "My Profile", false, null));
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
        //        //await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));
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