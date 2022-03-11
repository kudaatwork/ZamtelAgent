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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.Models.Image;
using YomoneyApp.Services;
using YomoneyApp.ViewModels.Geo;

namespace YomoneyApp.Views.Promotions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadPromotion : ContentPage
    {
        //public static readonly BindableProperty FocusOriginCommandProperty =
        //   BindableProperty.Create(nameof(FocusOriginCommand), typeof(ICommand), typeof(UploadPromotion), null, BindingMode.TwoWay);

        //public ICommand FocusOriginCommand
        //{
        //    get { return (ICommand)GetValue(FocusOriginCommandProperty); }
        //    set { SetValue(FocusOriginCommandProperty, value); }
        //}

        PromotionsViewModel promotionsViewModel;
        RequestViewModel viewModel;
        MainViewModel mainViewModel;
        private MediaFile _mediaFile;

        public UploadPromotion(MenuItem menuItem)
        {
            InitializeComponent();

            menuItem.Title = "Create Advert";
            BindingContext = promotionsViewModel = new PromotionsViewModel(this, menuItem);
            viewModel = new RequestViewModel(this);           

            AdvertPosition.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.AdPosition = AdvertPosition.Items[AdvertPosition.SelectedIndex];
            };

            AdvertType.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.Adtype = AdvertType.Items[AdvertType.SelectedIndex];
            };

            LinkType.SelectedIndexChanged += async (sender, e) =>
            {
                promotionsViewModel.LinkParameterName = LinkType.Items[LinkType.SelectedIndex];
                promotionsViewModel.HasParameter = true;
            };

            Sex.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.Sex = Sex.Items[Sex.SelectedIndex];
            };

            TargetAge.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.MinAge = int.Parse(TargetAge.Items[TargetAge.SelectedIndex]);
            };

            TargetAge2.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.MaxAge = int.Parse(TargetAge2.Items[TargetAge2.SelectedIndex]) ;
            };

            Currency.SelectedIndexChanged += (sender, e) =>
            {
                promotionsViewModel.Currency = Currency.Items[Currency.SelectedIndex];
            };
        }

        public UploadPromotion()
        {
        }

        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();

        //    if (BindingContext != null)
        //    {
        //        FocusOriginCommand = new Command(OnOriginFocus);
        //    }
        //}

        void OnOriginFocus()
        {
            Address.Focus();
        }
        private async void btnPickAdImage_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Photo Picked", "No Photo picked", "Ok");
            }

            _mediaFile = await CrossMedia.Current.PickPhotoAsync();

            if (_mediaFile == null)
                return;

            FileImage.Source = ImageSource.FromStream(() =>
            {
                return _mediaFile.GetStream();
            });

            btnSubmitPromotion.IsEnabled = true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            promotionsViewModel.IsBusy = true;

            var advertpositions = new List<string> { "HOME", "INAPP", "OWNPAGE", "EMAIL CAMPAIGN" };
            var advertTypes = new List<string> { "FLYER", "DISCOUNT PROMOTION", "SHAREADVERT", "SHARELINK", "PROMOTION", "SMS", "SELF" };
            var linkTypes = new List<string> { "Weblink" };
            var sexs = new List<string> { "Both", "Male", "Female" };
            var targetAges = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
                49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66,
                67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
                85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

            var targetAges2 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
                14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
                49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66,
                67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
                85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

            if (string.IsNullOrEmpty(promotionsViewModel.AdPosition))
            {
                AdvertPosition.Items.Clear();
                foreach (var advertposition in advertpositions)
                    AdvertPosition.Items.Add(advertposition);
            }

            if (string.IsNullOrEmpty(promotionsViewModel.Adtype))
            {
                AdvertType.Items.Clear();
                foreach (var advertType in advertTypes)
                    AdvertType.Items.Add(advertType);
            }

            if (string.IsNullOrEmpty(promotionsViewModel.LinkParameterName))
            {
                LinkType.Items.Clear();
                foreach (var linkType in linkTypes)
                    LinkType.Items.Add(linkType);
            }

            if (string.IsNullOrEmpty(promotionsViewModel.Sex))
            {
                Sex.Items.Clear();
                foreach (var sex in sexs)
                    Sex.Items.Add(sex);
            }

            if (promotionsViewModel.MinAge <= 0)
            {
                TargetAge.Items.Clear();
                foreach (var targetAge in targetAges)
                {
                    TargetAge.Items.Add(targetAge.ToString());                    
                }
            }

            if (promotionsViewModel.MaxAge <= 0)
            {
                TargetAge2.Items.Clear();
                foreach (var targetAge2 in targetAges2)
                {
                    TargetAge2.Items.Add(targetAge2.ToString());
                }
            }

            //if (promotionsViewModel.MinAge <= 0 || promotionsViewModel.MaxAge <= 0)
            //{
            //    TargetAge.Items.Clear();
            //    foreach (var targetAge in targetAges)
            //    {
            //        TargetAge.Items.Add(targetAge.ToString());
            //        TargetAge2.Items.Add(targetAge.ToString());
            //    }
            //}

            if (string.IsNullOrEmpty(promotionsViewModel.Currency))
            {
                var currencies = await viewModel.GetCurrenciesAsync();
                Currency.Items.Clear();
                foreach (var cur in currencies)
                    Currency.Items.Add(cur.Title.Trim());
            }

            promotionsViewModel.IsBusy = false;

            btnSubmitPromotion.IsEnabled = false;
        }

        private async void btnSubmitPromotion_Clicked(object sender, EventArgs e)
        {
            promotionsViewModel.IsBusy = true;
            promotionsViewModel.Message = "Loading...";

            if (string.IsNullOrEmpty(promotionsViewModel.Name))
            {
                await DisplayAlert("Advert Name Error!", "Please fill in the name", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(promotionsViewModel.Description))
            {
                await DisplayAlert("Advert Description Error!", "Please fill in the description", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(promotionsViewModel.AdPosition))
            {
                await DisplayAlert("Advert Advert Position Error!", "Please fill in the Ad Position field", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(promotionsViewModel.Adtype))
            {
                await DisplayAlert("Advert Advert Type Error!", "Please fill in the Ad Type field", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(promotionsViewModel.LinkParameterName))
            {
                await DisplayAlert("Advert Link Type Error!", "Please fill in the Link Type field", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            if (string.IsNullOrEmpty(promotionsViewModel.Sex))
            {
                await DisplayAlert("Advert Gender Error!", "Please fill in the Gender field", "Ok");
                promotionsViewModel.IsBusy = false;
                return;
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("http://102.130.120.163:8058/");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new MultipartFormDataContent();

                    content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");

                    var uploadServiceBaseAddress = "api/Files/Upload";

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                    var response = httpResponseMessage.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<string>(response.Result);

                    if (result.ToUpper() == "FAILED")
                    {
                        await DisplayAlert("Advert Upload Error!", "There was an error saving the Ad. Please try again", "OK");
                        promotionsViewModel.IsBusy = false;
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

                        fileUpload.Name = finalFileName;
                        fileUpload.Type = type;
                        fileUpload.PhoneNumber = uname;
                        fileUpload.Image = "http://102.130.120.163:8058" + result;
                        fileUpload.Purpose = "Advert";
                        fileUpload.ServiceId = 0;
                        fileUpload.ActionId = 0;

                        var view = sender as Xamarin.Forms.Button;
                        Advert advert = new Advert();

                        var x = JsonConvert.SerializeObject(view.CommandParameter);
                        advert = JsonConvert.DeserializeObject<Advert>(x);

                        var promotion = JsonConvert.SerializeObject(advert);

                        fileUpload.SupplierId = promotion;

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

                               // var stringResult = JsonConvert.DeserializeObject<string>(result);

                                if (serverresult == "Success")
                                {
                                    await DisplayAlert("Advert Upload Success!", "Advert has been submitted successfully and is up for a review", "OK");
                                    //await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));

                                    AdName = null;
                                   // promotionsViewModel.Name = null;

                                    promotionsViewModel.Description = null;
                                    //AdDescription = null;

                                    promotionsViewModel.AdPosition = null;
                                    //AdvertPosition = null;

                                    promotionsViewModel.Adtype = null;
                                    //AdvertType = null;

                                    LinkType = null;

                                    PageUrl = null;
                                    //promotionsViewModel.SiteUrl = null;

                                    Sex = null;

                                    TargetAge = null;
                                    //promotionsViewModel.MinAge = 0;

                                    TargetAge2 = null;
                                    //promotionsViewModel.MaxAge = 0;

                                    ExpDate = null;
                                    //promotionsViewModel.ExpireryDate = DateTime.Now.Date;

                                    Currency = null;
                                    //promotionsViewModel.Currency = null;

                                    MaximumDailyBudget = null;

                                    Address = null;

                                    AreaRadius = null;

                                    FileImage = null;

                                    MenuItem mn = new YomoneyApp.MenuItem();
                                    mn.Title = "My Promotions";
                                    mn.TransactionType = 23;
                                    mn.Section = "PROMOTIONS";
                                    mn.SupplierId = "All";

                                    await Navigation.PushAsync(new MyPromotions(mn));
                                }
                                else
                                {
                                    await DisplayAlert("Advert Upload Error!", "There was an error saving the Ad. Please try again", "OK");
                                    promotionsViewModel.IsBusy = false;
                                }
                                //FileImage.Source.ClearValue();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace + ex.InnerException);

                    await DisplayAlert("Advert Upload Error!", "There was an error saving the Ad. Please try again", "OK");
                    promotionsViewModel.IsBusy = false;
                }
            }

            #region Commented Out Code

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

            //// FileInfo fileDetail = new FileInfo(fileName);

            ////if (fileDetail.Length > 2097152)
            ////{
            ////    await DisplayAlert("File Too Large Error!", "File cannot exceed 2MB", "Ok");
            ////}
            ////else
            ////{
            //fileUpload.Name = fileName;
            //fileUpload.Type = type;
            //fileUpload.PhoneNumber = uname;
            //fileUpload.Image = base64;
            //fileUpload.Purpose = "Advert";
            //fileUpload.ServiceId = 0;
            //fileUpload.ActionId = 0;

            //var view = sender as Xamarin.Forms.Button;
            //Advert advert = new Advert();

            //var x = JsonConvert.SerializeObject(view.CommandParameter);
            //advert = JsonConvert.DeserializeObject<Advert>(x);            

            //var promotion = JsonConvert.SerializeObject(advert);

            //fileUpload.SupplierId = promotion;

            //// var json = JsonConvert.SerializeObject(fileUpload);

            //string Body = string.Empty;

            //Body += "Product=" + base64;

            ////HttpClient client = new HttpClient();
            ////var myContent = Body;
            ////string paramlocal = string.Format("https://www.yomoneyservice.com/Mobile/Transaction/?{0}", myContent);
            ////string resultt = await client.GetStringAsync(paramlocal);
            ////if (resultt != "System.IO.MemoryStream")
            ////{
            ////    var stringResult = JsonConvert.DeserializeObject<string>(resultt);

            ////    if (stringResult == "")
            ////    {

            ////    }
            ////}

            //TransactionRequest transactionRequest = new TransactionRequest();

            //transactionRequest.AgentCode = "admin@cfs.co.zw:cfs6778";
            //transactionRequest.MTI = "0100";
            ////transactionRequest.Note = base64;

            //HttpClient client = new HttpClient();

            //Uri uri = new Uri("https://www.yomoneyservice.com/Mobile/FileUploader");
            ////string url = String.Format("https://www.yomoneyservice.com/yoclient/transaction");
            //var payload = JsonConvert.SerializeObject(fileUpload);
            //HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            //HttpResponseMessage result = await client.PostAsync(uri, httpContent);

            //string response = await result.Content.ReadAsStringAsync();
            //saved = false;

            ////try
            ////{
            ////    string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
            ////    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            ////    httpWebRequest.ContentType = "application/json";
            ////    httpWebRequest.Method = "POST";
            ////    httpWebRequest.Timeout = 120000;
            ////    httpWebRequest.CookieContainer = new CookieContainer();
            ////    Cookie cookie = new Cookie("AspxAutoDetectCookieSupport", "1");
            ////    cookie.Domain = "www.yomoneyservice.com";
            ////    httpWebRequest.CookieContainer.Add(cookie);

            ////    var json = JsonConvert.SerializeObject(fileUpload);

            ////    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            ////    {
            ////        streamWriter.Write(json);
            ////        streamWriter.Flush();
            ////        streamWriter.Close();

            ////        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            ////        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            ////        {
            ////            var result = streamReader.ReadToEnd();

            ////            var stringResult = JsonConvert.DeserializeObject<string>(result);

            ////            if (stringResult == "Success")
            ////            {
            ////                promotionsViewModel.IsBusy = false;
            ////                promotionsViewModel.Message = "";
            ////                saved = true;
            ////            }
            ////            else
            ////            {
            ////                promotionsViewModel.IsBusy = false;
            ////                promotionsViewModel.Message = "";
            ////                saved = false;
            ////            }
            ////            //FileImage.Source.ClearValue();

            ////        }
            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    promotionsViewModel.IsBusy = false;s
            ////    promotionsViewModel.Message = "";
            ////    Console.WriteLine(ex.Message);
            ////    saved = false;
            ////}
            ////}           

            //if (saved)
            //{
            //    await DisplayAlert("Advert Upload Success!", "Advert has been submitted successfully and is up for a review", "OK");
            //    //await Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com/Mobile/JobProfile?id=" + uname, "My Profile", false,null));

            //    MenuItem mn = new YomoneyApp.MenuItem();
            //    mn.Title = "My Promotions";
            //    mn.TransactionType = 23;
            //    mn.Section = "PROMOTIONS";
            //    mn.SupplierId = "All";

            //    await Navigation.PushAsync(new MyPromotions(mn));
            //}
            //else
            //{
            //    await DisplayAlert("Advert Upload Error!", "There was an error saving the Ad. Please try again", "OK");
            //    promotionsViewModel.IsBusy = false;
            //}

            #endregion
        }
        
    }
}