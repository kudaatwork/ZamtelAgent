using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SignaturePad.Forms;
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
using YomoneyApp.Interfaces;
using YomoneyApp.Models.Agent;
using YomoneyApp.Models.AuthenticationDetail;
using YomoneyApp.ViewModels.Login;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgentRegistrationThree : ContentPage
    {
        AccountViewModel viewModel;
        private MediaFile _portraitFile;
        private MediaFile _nicFrontFile;
        private MediaFile _nicBackFile;        
        private FileResult _agentContractFile;

        public AgentRegistrationThree()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);
        }

        private void Camera_Clicked(object sender, EventArgs e)
        {

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignIn());
        }

        private async void Portrait_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }

            _portraitFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = DateTime.Now.ToString("yyyyMMddHHmmssffffff"),
                CompressionQuality = 60
            });

            if (_portraitFile == null) return;

            PortraitImage.Source = ImageSource.FromStream(() =>
            {
                return _portraitFile.GetStream();
            });

            Portrait.Text = "UPDATE PORTRAIT";

            if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
            {
                SavePhoto.IsEnabled = true;
            }
        }

        private async void NationalIdFront_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }

            _nicFrontFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = DateTime.Now.ToString("yyyyMMddHHmmssffffff"),
                CompressionQuality = 60
            });

            if (_nicFrontFile == null) return;

            NationalIdFrontImage.Source = ImageSource.FromStream(() =>
            {
                return _nicFrontFile.GetStream();
            });

            NationalIdFront.Text = "UPDATE NIC FRONT";

            if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
            {
                SavePhoto.IsEnabled = true;
            }
        }

        private async void NationalIdBack_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }

            _nicBackFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = DateTime.Now.ToString("yyyyMMddHHmmssffffff"),
                CompressionQuality = 60
            });

            if (_nicBackFile == null) return;

            NationalIdBackImage.Source = ImageSource.FromStream(() =>
            {
                return _nicBackFile.GetStream();
            });

            NationalIdBack.Text = "UPDATE NIC BACK";

            if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
            {
                SavePhoto.IsEnabled = true;
            }
        }

        private async void Signature_Clicked(object sender, EventArgs e)
        {
            if (signatureView.IsEnabled == true)
            {
                signatureView.IsEnabled = false;
                Signature.Text = "EDIT SIGNATURE";

                if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
                {
                    SavePhoto.IsEnabled = true;
                }
            }
            else
            {
                signatureView.IsEnabled = true;
                Signature.Text = "SAVE SIGNATURE";

                if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
                {
                    SavePhoto.IsEnabled = true;
                }
            }
        }

        private async void AgentContract_Clicked(object sender, EventArgs e)
        {
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

            _agentContractFile = await FilePicker.PickAsync(options);

            if (_agentContractFile != null)
            {
                char[] delimite = new char[] { '.' };

                string[] parts = _agentContractFile.FileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                if (parts[1] == "png" || parts[1] == "jpg" || parts[1] == "jpeg")
                {
                    if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
                    {
                        SavePhoto.IsEnabled = true;
                    }

                    FileStream fileStream = new FileStream(_agentContractFile.FullPath, FileMode.Open, FileAccess.Read);

                    AgentContractImage.Source = ImageSource.FromStream(() =>
                    {
                        return fileStream;
                    });
                }
                else
                {
                    if (PortraitImage.Source != null && NationalIdBackImage.Source != null && NationalIdBackImage.Source != null)
                    {
                        SavePhoto.IsEnabled = true;
                    }

                    AgentContractImage.Source = "attachment1.png";
                }
            }

            AgentContract.Text = "UPDATE AGENT CONTRACT";
        }

        private async void SavePhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var authenticationDetail = new AuthenticationDetail();

                using (var httpClient = new HttpClient(Xamarin.Forms.DependencyService.Get<IMyOwnNetService>().GetHttpClientHandler()))
                {
                    using (var bitmap = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png))
                    {
                        string authInfo = authenticationDetail.Username + ":" + authenticationDetail.Password;
                        authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                        httpClient.BaseAddress = new Uri("https://192.168.100.172:45455");
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

                        var content = new MultipartFormDataContent();

                        var agent = new Agent()
                        {
                            Firstname = AccountViewModel.Firstname,
                            Middlename = AccountViewModel.Middlename,
                            Lastname = AccountViewModel.Lastname,
                            DeviceOwnership = AccountViewModel.DeviceOwnership,
                            SupervisorId = AccountViewModel.SupervisorId,
                            Password = AccountViewModel.Password,
                            Gender = AccountViewModel.Gender,
                            Area = AccountViewModel.Area,
                            TownId = AccountViewModel.TownId,
                            ProvinceId = AccountViewModel.ProvinceId,
                            NationalityId = AccountViewModel.NationalityId,
                            IdNumber = AccountViewModel.IdNumber,
                            MobileNumber = AccountViewModel.MobileNumber,
                            AlternativeMobileNumber = AccountViewModel.AlternativeMobileNumber,
                            AgentCode = AccountViewModel.AgentCode
                        };

                        var json = JsonConvert.SerializeObject(agent);

                        FileStream fileStream = new FileStream(_agentContractFile.FullPath, FileMode.Open, FileAccess.Read);

                        content.Add(new StreamContent(_portraitFile.GetStream()), "\"Portrait\"", $"\"{_portraitFile.Path}\"");
                        content.Add(new StreamContent(_nicFrontFile.GetStream()), "\"NationalIdFront\"", $"\"{_nicFrontFile.Path}\"");
                        content.Add(new StreamContent(_nicBackFile.GetStream()), "\"NationalIdBack\"", $"\"{_nicBackFile.Path}\"");
                        content.Add(new StreamContent(fileStream), "\"AgentContractForm\"", $"\"{_agentContractFile.FullPath}\"");
                        content.Add(new StreamContent(bitmap), "\"Signature\"", $"\"{DateTime.Now.ToString("yyyyMMddHHmmssffffff")}\"");
                        content.Add(new StringContent(json, Encoding.UTF8, "application/json"));

                        var uploadServiceBaseAddress = "api/agents";

                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        httpClient.Timeout = TimeSpan.FromMinutes(3);

                        var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                        var response = httpResponseMessage.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<string>(response.Result);

                        if (result == "")
                        {

                        }
                    }                                        
                }                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                await DisplayAlert("Error!", "Processing Error. Please, try again.", "Ok");
            }

        }

       
        private void SignatureChanged(object sender, EventArgs e)
        {
            
        }
    }
}