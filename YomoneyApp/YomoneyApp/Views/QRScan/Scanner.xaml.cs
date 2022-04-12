﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;
using YomoneyApp.Views.Webview;
using ZXing;

namespace YomoneyApp.Views.QRScan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Scanner : ContentPage
    {
        string HostDomain = "http://192.168.100.150:5000";
        string webviewLink = "/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&serviceId=" + HomeViewModel.fileUpload.ServiceId + "&ActionId=" + HomeViewModel.fileUpload.ActionId +
            "&FormNumber=" + HomeViewModel.fileUpload.FormId + "&Customer=" + HomeViewModel.fileUpload.PhoneNumber + "&CallType=FirstTime";
        string title = "";

        public Scanner()
        {
            InitializeComponent();

            ScannerView.Options = new ZXing.Mobile.MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.DATA_MATRIX,
                    BarcodeFormat.PDF_417,
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.MAXICODE,
                    BarcodeFormat.UPC_EAN_EXTENSION
                },
                TryHarder = true,
                AutoRotate = false,
                TryInverted = true,
                DelayBetweenContinuousScans = 1000,
            };

            ScannerView.OnScanResult += async (result) =>
            {
                // var x = 3; // Breakpoint here, never hit
                if (ScannerView.IsScanning)
                {
                    ScannerView.AutoFocus();
                }

                try
                {
                    AccessSettings acnt = new AccessSettings();
                    string pass = acnt.Password;
                    string uname = acnt.UserName;

                    FileUpload fileUpload = new FileUpload();

                    fileUpload.Name = result.Text;
                    //fileUpload.Type = "";
                    fileUpload.PhoneNumber = uname;
                    //fileUpload.Image = "";
                    fileUpload.Purpose = "FORMS";
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
                                
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    //viewModel.IsBusy = false;
                                    //FileImage.Source = null;

                                    await DisplayAlert("File Upload", "File scanned and saved successfully", "OK");

                                    Navigation.PopAsync();

                                    await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "File Upload", false, null));
                                });
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(async () => 
                                {
                                    await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                                    //viewModel.IsBusy = false;

                                });
                              
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }               
            };

            ScannerOverlay.ShowFlashButton = ScannerView.HasTorch;
            ScannerOverlay.FlashButtonClicked += (se, ev) => ScannerView.ToggleTorch();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ScannerView.IsAnalyzing = true;
            ScannerView.IsScanning = true;
        }

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();

        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + webviewLink, title, false, null, false));
        //    });
        //}

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ScannerView.IsScanning = false;

            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    Navigation.PopAsync();
            //    await Navigation.PushAsync(new WebviewHyubridConfirm(HostDomain + webviewLink, title, false, null, false));
            //});
        }

        public async void SaveScan(string scanPath)
        {

            ScannerView.OnScanResult += async (result) =>
            {
                // var x = 3; // Breakpoint here, never hit
                if (ScannerView.IsScanning)
                {
                    ScannerView.AutoFocus();
                }

                AccessSettings acnt = new AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                FileUpload fileUpload = new FileUpload();
               
                fileUpload.Name = scanPath;
                //fileUpload.Type = "";
                fileUpload.PhoneNumber = uname;
                //fileUpload.Image = "";
                fileUpload.Purpose = "FIELD";
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
                            await DisplayAlert("File Upload", "File scanned and saved successfully", "OK");

                            //viewModel.IsBusy = false;
                            //FileImage.Source = null;

                            await Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "File Upload", false, null));

                            //Device.BeginInvokeOnMainThread(async () =>
                            //{
                            //    await App.Current.MainPage.Navigation.PushAsync(new HomePage());
                            //});
                        }
                        else
                        {
                            await DisplayAlert("File Upload", "There was an error saving the image.", "OK");
                            //viewModel.IsBusy = false;
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
            };            
        }        
    }
}