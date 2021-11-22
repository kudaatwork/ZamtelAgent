using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
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

namespace YomoneyApp.Views.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalDetails : ContentPage
    {
        WalletServicesViewModel viewModel;
        private MediaFile _mediaFile;
        private MediaFile _takenFile;

        public PersonalDetails(string loyalty, string services, string tasks)
        {
            InitializeComponent();
            BindingContext = viewModel = new YomoneyApp.WalletServicesViewModel(this);
            viewModel.LoyaltySchemes = loyalty;
            viewModel.Services = services;
            viewModel.Tasks = tasks;

            AccessSettings accessSettings = new AccessSettings();

            viewModel.PhoneNumber = accessSettings.UserName;

            Gender.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.Gender = Gender.Items[Gender.SelectedIndex];
            };

            Countries.SelectedIndexChanged += (sender, e) =>
            {
                viewModel.ActiveCountry = Countries.Items[Countries.SelectedIndex];
            };
        }

        public static string[] countries = new string[]
        {
            "Afghanistan",
            "Albania",
            "Algeria",
            "American Samoa",
            "Andorra",
            "Angola",
            "Anguilla",
            "Antarctica",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Aruba",
            "Australia",
            "Austria",
            "Azerbaijan",
            "Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bermuda",
            "Bhutan",
            "Bolivia",
            "Bosnia and Herzegovina",
            "Botswana",
            "Bouvet Island",
            "Brazil",
            "British Indian Ocean Territory",
            "Brunei Darussalam",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Cape Verde",
            "Cayman Islands",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Christmas Island",
            "Cocos (Keeling) Islands",
            "Colombia",
            "Comoros",
            "Congo",
            "Congo, the Democratic Republic of the",
            "Cook Islands",
            "Costa Rica",
            "Cote D'Ivoire",
            "Croatia",
            "Cuba",
            "Cyprus",
            "Czech Republic",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Dominican Republic",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Ethiopia",
            "Falkland Islands (Malvinas)",
            "Faroe Islands",
            "Fiji",
            "Finland",
            "France",
            "French Guiana",
            "French Polynesia",
            "French Southern Territories",
            "Gabon",
            "Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Gibraltar",
            "Greece",
            "Greenland",
            "Grenada",
            "Guadeloupe",
            "Guam",
            "Guatemala",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Haiti",
            "Heard Island and Mcdonald Islands",
            "Holy See (Vatican City State)",
            "Honduras",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran, Islamic Republic of",
            "Iraq",
            "Ireland",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "Korea, Democratic People's Republic of",
            "Korea, Republic of",
            "Kuwait",
            "Kyrgyzstan",
            "Lao People's Democratic Republic",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libyan Arab Jamahiriya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Macao",
            "Macedonia, the Former Yugoslav Republic of",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Martinique",
            "Mauritania",
            "Mauritius",
            "Mayotte",
            "Mexico",
            "Micronesia, Federated States of",
            "Moldova, Republic of",
            "Monaco",
            "Mongolia",
            "Montserrat",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "Netherlands Antilles",
            "New Caledonia",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "Niue",
            "Norfolk Island",
            "Northern Mariana Islands",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Palestinian Territory, Occupied",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Philippines",
            "Pitcairn",
            "Poland",
            "Portugal",
            "Puerto Rico",
            "Qatar",
            "Reunion",
            "Romania",
            "Russian Federation",
            "Rwanda",
            "Saint Helena",
            "Saint Kitts and Nevis",
            "Saint Lucia",
            "Saint Pierre and Miquelon",
            "Saint Vincent and the Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudi Arabia",
            "Senegal",
            "Serbia and Montenegro",
            "Seychelles",
            "Sierra Leone",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "South Georgia and the South Sandwich Islands",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Suriname",
            "Svalbard and Jan Mayen",
            "Swaziland",
            "Sweden",
            "Switzerland",
            "Syrian Arab Republic",
            "Taiwan, Province of China",
            "Tajikistan",
            "Tanzania, United Republic of",
            "Thailand",
            "Timor-Leste",
            "Togo",
            "Tokelau",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Turks and Caicos Islands",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "United States",
            "United States Minor Outlying Islands",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Venezuela",
            "Viet Nam",
            "Virgin Islands, British",
            "Virgin Islands, US",
            "Wallis and Futuna",
            "Western Sahara",
            "Yemen",
            "Zambia",
            "Zimbabwe",
        };

        protected override async void OnAppearing()
        {
            List<string> gender = new List<string>();

            gender.Add("MALE");
            gender.Add("FEMALE");

            List<string> cntrs = new List<string>(countries);

            base.OnAppearing();

            try
            {
                ButtonSubmitFeedback.IsEnabled = false;

                foreach (var item in gender)
                    Gender.Items.Add(item.Trim());

                foreach (var item2 in cntrs)
                    Countries.Items.Add(item2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Error!", "Unable to gather billers because of a server error. Contact customer support", "OK");
            }

        }

        private async void btnTakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available", "Ok");
            }

            _takenFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "MyImage.jpg"
            });

            if (_takenFile == null) return;

            FileTaken.Source = ImageSource.FromStream(() =>
            {
                return _takenFile.GetStream();
            });

            ButtonSubmitFeedback.IsEnabled = true;
        }

        private async void btnPickPhoto_Clicked(object sender, EventArgs e)
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

            ButtonSubmitFeedback.IsEnabled = true;
        }

        private async void ButtonSubmitFeedback_Clicked(object sender, EventArgs e)
        {
            ButtonSubmitFeedback.IsEnabled = false;
            ButtonSubmitFeedback.Text = "PROCESSING...";

            if (!viewModel.PersonalDetails)
            {
                var response = viewModel.ExecuteSubmitPersonalDetailsCommand(); // Submit Details First
            }
            else
            {
                PostId();
            }
            
            MessagingCenter.Subscribe<string, string>("VerifyCustomer", "PersonalData", async (sender, arg) =>
            {
                if (arg.ToUpper().Trim() == "SUCCESS")
                {
                    viewModel.IsBusy = true;
                    viewModel.Message = "Personal Details Submitted successfull";
                    viewModel.PersonalDetails = true;

                    PostId();   
                }
                else
                {
                    viewModel.PersonalDetails = false;
                    await DisplayAlert("Error!", "There has been an error in submitting your personal details to the server", "OK");
                }

               // MessagingCenter.Unsubscribe<string, string>("VerifyCustomer", "PersonalData");
            });            
        }

        private async void PostId()
        {
            if (!viewModel.IdImage)
            {
                bool saved = false;
                FileUpload fileUpload = new FileUpload();

                var stream = _mediaFile.GetStream();

                if (stream != null)
                {
                    var bytes = new byte[stream.Length];

                    stream.Read(bytes, 0, (int)stream.Length); // Uploaded File
                    string base64 = System.Convert.ToBase64String(bytes);

                    string strPath = _mediaFile.Path; // Uploaded File Path

                    string fileName = Path.GetFileName(strPath);

                    char[] delimite = new char[] { '.' };

                    string[] parts = fileName.Split(delimite, StringSplitOptions.RemoveEmptyEntries); // filename of Uploaded File

                    string type = parts[1];

                    fileUpload.Name = fileName;

                    fileUpload.Type = type;

                    fileUpload.PhoneNumber = "263784607691";

                    fileUpload.Image = base64;

                    fileUpload.Purpose = "VerifyId";

                    try
                    {
                        string url = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Timeout = 120000;

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

                                var desirializedResponse = JsonConvert.DeserializeObject<string>(result);

                                if (desirializedResponse.ToUpper().Trim() == "SUCCESS")
                                {
                                    viewModel.IsBusy = true;
                                    viewModel.Message = "Id uploaded successfully";
                                    saved = true;

                                    viewModel.IdImage = true;
                                }
                                else
                                {
                                    viewModel.IdImage = false;
                                    saved = false;                                    
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                        viewModel.IdImage = false;
                        ButtonSubmitFeedback.IsEnabled = true;
                        ButtonSubmitFeedback.Text = "RETRY UPLOADING ID";
                    }

                    if (saved && viewModel.TakenImage == false)
                    {
                        PostImageTaken();
                    }
                    else
                    {
                        ButtonSubmitFeedback.IsEnabled = true;
                        ButtonSubmitFeedback.Text = "RETRY UPLOADING ID";
                    }
                }
                else
                {
                    await DisplayAlert("Error!", "No Image was selected", "OK");
                }
            }
            else
            {
                PostImageTaken();
            }
        }

        private void PostImageTaken()
        {
            if (!viewModel.TakenImage)
            {
                bool saved = false;

                FileUpload fileUpload2 = new FileUpload();

                var stream2 = _takenFile.GetStream();

                var bytes2 = new byte[stream2.Length];

                var strPath2 = _takenFile.Path; // Taken File Path

                var fileName2 = Path.GetFileName(strPath2); // filename of Taken File

                stream2.Read(bytes2, 0, (int)stream2.Length); // File Taken

                var strbase64 = System.Convert.ToBase64String(bytes2);

                char[] delimite2 = new char[] { '.' };

                var parts2 = fileName2.Split(delimite2, StringSplitOptions.RemoveEmptyEntries); // filename of Taken File

                var fileType = parts2[1]; //taken

                fileUpload2.Name = fileName2;

                fileUpload2.Type = fileType;

                fileUpload2.PhoneNumber = "263784607691";

                fileUpload2.Image = strbase64;

                fileUpload2.Purpose = "VerifyImage";

                try
                {
                    string url2 = String.Format("https://www.yomoneyservice.com/Mobile/FileUploader");
                    var httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url2);
                    httpWebRequest2.ContentType = "application/json";
                    httpWebRequest2.Method = "POST";
                    httpWebRequest2.Timeout = 120000;

                    var json2 = JsonConvert.SerializeObject(fileUpload2);

                    using (var streamWriter2 = new StreamWriter(httpWebRequest2.GetRequestStream()))
                    {
                        streamWriter2.Write(json2);
                        streamWriter2.Flush();
                        streamWriter2.Close();

                        var httpResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();

                        using (var streamReader2 = new StreamReader(httpResponse2.GetResponseStream()))
                        {
                            var result = streamReader2.ReadToEnd();

                            var deserializedResponse = JsonConvert.DeserializeObject<string>(result);

                            if (deserializedResponse.ToUpper().Trim() == "SUCCESS")
                            {
                                viewModel.IsBusy = true;
                                viewModel.Message = "Taken Image uploaded Successfully";

                                saved = true;
                                viewModel.TakenImage = true;

                                IsBusy = false;

                                ButtonSubmitFeedback.IsEnabled = false;
                                ButtonSubmitFeedback.Text = "DONE";

                                DisplayAlert("Success!", "Personal Details updated successfully. Your account is now undergoing a verification process.", "OK");
                               
                                Navigation.PushAsync(new WaletServices(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));

                            }
                            else
                            {
                                saved = false;
                                viewModel.TakenImage = false;
                                IsBusy = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    viewModel.IdImage = false;

                    IsBusy = false;

                    ButtonSubmitFeedback.IsEnabled = true;
                    ButtonSubmitFeedback.Text = "RETRY UPLOADING TAKEN IMAGE";
                }

                if (!saved)
                {

                    IsBusy = false;
                    ButtonSubmitFeedback.IsEnabled = true;
                    ButtonSubmitFeedback.Text = "RETRY UPLOADING TAKEN IMAGE";
                }
            }
            else
            {
                DisplayAlert("Success!", "It seems like your Personal Details were updated successfully. Your account is now undergoing a verification process.", "OK");

                Navigation.PushAsync(new WaletServices(viewModel.LoyaltySchemes, viewModel.Services, viewModel.Tasks));
            }
        }        

        private void MyDataPicker_DateSelected(object sender, DateChangedEventArgs e)
        {

        }        
    }
}