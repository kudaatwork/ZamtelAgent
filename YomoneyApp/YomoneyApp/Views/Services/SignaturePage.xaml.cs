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
using Newtonsoft.Json;
using System.IO;
using System.Net;
using YomoneyApp.Models.Image;
using YomoneyApp.Views.Webview;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignaturePage : ContentPage
    {
        string HostDomain = "http://192.168.100.150:5000";

        MenuItem SelectedItem;
        private Point[] points;
        private MediaFile _mediaFile;
        ServiceViewModel viewModel;

        AccountViewModel accountViewModel = new AccountViewModel(null);

        public SignaturePage(MenuItem mnu = null)
        {
            InitializeComponent();
            BindingContext = viewModel = new ServiceViewModel(this, mnu);
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
            viewModel.IsBusy = true;
            bool saved = false;
            FileUpload fileUpload = new FileUpload();

            try
            {
                using (var bitmap = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png))
                {

                    //using (FileStream file = new FileStream("", FileMode.Create, System.IO.FileAccess.Write))
                    //{
                    //    bitmap.CopyTo(file);
                    //}

                    var bytes = new byte[bitmap.Length];
                    await bitmap.ReadAsync(bytes, 0, (int)bitmap.Length);
                    string base64 = System.Convert.ToBase64String(bytes);
                    fileUpload.Image = base64;

                    //var fileName1 = "sigFile";

                    #region Upload Signature to server

                    var httpClient = new HttpClient();

                    httpClient.BaseAddress = new Uri("http://102.130.120.163:8058/");
                    // httpClient.DefaultRequestHeaders.Accept.Clear();
                    // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // var content = new MultipartFormDataContent();

                    // content.Add(new StreamContent(bitmap), "\"file\"", $"\"Content\\Uploads\"");

                    var json1 = JsonConvert.SerializeObject(fileUpload);
                    var data = new StringContent(json1, Encoding.UTF8, "application/json");

                    var uploadServiceBaseAddress = "api/Signature/Upload";

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    httpClient.Timeout = TimeSpan.FromMinutes(3);

                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, data);

                    var response = httpResponseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<string>(response.Result);

                    //var result = response.Result;

                    if (response.Result.ToUpper() == "FAILED")
                    {
                        await DisplayAlert("File Upload", "There was an error saving the signature.", "OK");
                        viewModel.IsBusy = false;
                    }
                    else
                    {
                        AccessSettings acnt = new AccessSettings();
                        string pass = acnt.Password;
                        string uname = acnt.UserName;

                        //var bytes = new byte[bitmap.Length];
                        //await bitmap.ReadAsync(bytes, 0, (int)bitmap.Length);
                        //string base64 = System.Convert.ToBase64String(bytes);

                        char[] delimite = new char[] { '/' };

                        string[] parts1 = result.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                        var fileName = parts1[2];

                        char[] delimiter = new char[] { '.' };

                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        var type = parts[1];

                        //string regExp = "[^a-zA-Z0-9]";

                        var finalFileName = fileName.Replace(" ", "_");

                        fileUpload.Name = finalFileName;
                        fileUpload.Type = "png";
                        fileUpload.PhoneNumber = HomeViewModel.fileUpload.PhoneNumber;
                        fileUpload.Image = "http://102.130.120.163:8058" + result;                        

                        if (string.IsNullOrEmpty(HomeViewModel.fileUpload.SupplierId))
                        {
                            var formString = SelectedItem.Section.Replace("/Mobile/Forms?", "");

                            char[] delimiter2 = new char[] { '&' };

                            string[] parts2 = formString.Split(delimiter2, StringSplitOptions.RemoveEmptyEntries);

                            if (parts2[0].Contains("SupplierId"))
                            {
                                var supplierId = parts2[0].Trim().Replace("SupplierId=", "");
                                fileUpload.SupplierId = supplierId;
                            }                            
                        }
                        else
                        {
                            fileUpload.SupplierId = HomeViewModel.fileUpload.SupplierId;
                        }

                        if (HomeViewModel.fileUpload.ActionId == 0)
                        {
                            var formString = SelectedItem.Section.Replace("/Mobile/Forms?", "");

                            char[] delimiter2 = new char[] { '&' };

                            string[] parts2 = formString.Split(delimiter2, StringSplitOptions.RemoveEmptyEntries);

                            if (parts2[0].Contains("ActionId"))
                            {
                                var actionId = parts2[0].Trim().Replace("ActionId=", "");
                                fileUpload.ActionId = long.Parse(actionId);
                            }
                        }
                        else
                        {
                            fileUpload.ActionId = HomeViewModel.fileUpload.ActionId;
                        }

                        if (string.IsNullOrEmpty(HomeViewModel.fileUpload.FormId))
                        {
                            var formString = SelectedItem.Section.Replace("/Mobile/Forms?", "");

                            char[] delimiter2 = new char[] { '&' };

                            string[] parts2 = formString.Split(delimiter2, StringSplitOptions.RemoveEmptyEntries);

                            if (parts2[0].Contains("FormId"))
                            {
                                var formId = parts2[0].Trim().Replace("FormId=", "");
                                fileUpload.FormId = formId;
                            }
                        }
                        else
                        {
                            fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                        }

                        fileUpload.ServiceId = HomeViewModel.fileUpload.ServiceId;
                        fileUpload.Purpose = HomeViewModel.fileUpload.Purpose;
                        fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                        fileUpload.FieldId = HomeViewModel.fileUpload.FieldId;

                        try
                        {
                            string url = String.Format("http://192.168.100.150:5000/Mobile/FileUploader?user=" + uname + ":" + pass + "&upType=Signature");
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Timeout = 120000;
                            //httpWebRequest.CookieContainer = new CookieContainer();
                            //Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
                            //cookie.Domain = "http://192.168.100.150:5000";
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
                                    var result2 = streamReader.ReadToEnd();

                                    var resultResponse = JsonConvert.DeserializeObject<string>(result2);

                                    if (!resultResponse.Contains("Error"))
                                    {
                                        viewModel.IsBusy = false;
                                        await DisplayAlert("Signature Pad", "Raster signature saved to the photo library.", "OK");
                                        await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + "/" + resultResponse, "Signature Form", false, null, true));
                                    }
                                    else
                                    {                                        
                                        await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                                        viewModel.IsBusy = false;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                            await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                            viewModel.IsBusy = false;
                        }
                    }

                    #endregion                    

                    //if (saved)
                    //{
                    //    await DisplayAlert("Signature Pad", "Raster signature saved to the photo library.", "OK");
                    //    //await viewModel.ExecuteRenderActionCommand(null);
                    //}
                    //else
                    //{
                    //    await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                    //}                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SignatureChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }
    }
}