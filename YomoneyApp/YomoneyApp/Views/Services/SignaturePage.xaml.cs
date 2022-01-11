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

namespace YomoneyApp.Views.Services
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignaturePage : ContentPage
    {
        string HostDomain = "https://www.yomoneyservice.com";

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
            bool saved = false;
            FileUpload fileUpload = new FileUpload();

            try
            {
                using (var bitmap = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png))
                {
                    AccessSettings acnt = new AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;

                    var bytes = new byte[bitmap.Length];
                    await bitmap.ReadAsync(bytes, 0, (int)bitmap.Length);
                    string base64 = System.Convert.ToBase64String(bytes);

                    var fileName1 = "sigFile";

                    var fileName2 = DateTime.Now.ToString("ddMMyyHHmmss");

                    var fileName = fileName1 + "_" + fileName2;

                    fileUpload.Name = fileName;
                    fileUpload.Type = "png";
                    fileUpload.PhoneNumber = HomeViewModel.fileUpload.PhoneNumber;
                    fileUpload.Image = base64;
                    fileUpload.ServiceId = HomeViewModel.fileUpload.ServiceId;
                    fileUpload.ActionId = HomeViewModel.fileUpload.ActionId;
                    fileUpload.SupplierId = HomeViewModel.fileUpload.SupplierId;
                    fileUpload.Purpose = HomeViewModel.fileUpload.Purpose;
                    fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                    fileUpload.FieldId = HomeViewModel.fileUpload.FieldId;

                    try
                    {
                        string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader?user=" + uname + ":" + pass + "&upType=Signature");
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

                                var resultResponse = JsonConvert.DeserializeObject<string>(result);

                                if (!resultResponse.Contains("Error"))
                                {
                                    await DisplayAlert("Signature Pad", "Raster signature saved to the photo library.", "OK");
                                    await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + "/" + resultResponse, "Signature Form", false, null,true));
                                }
                                else
                                {
                                    await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        await DisplayAlert("Signature Pad", "There was an error saving the signature.", "OK");
                    }

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