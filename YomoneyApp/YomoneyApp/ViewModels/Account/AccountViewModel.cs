using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.DateTime;
using RetailKing.Models;
using System.Net.Http;
using Newtonsoft.Json;
using YomoneyApp.Services;
using YomoneyApp.Views.Login;
using YomoneyApp.ViewModels.Login;
using YomoneyApp.Models.Questions;
using System.Text.RegularExpressions;
using FluentValidation;
using System.Net;
using System.Linq;
using Xamarin.Essentials;
using System.Web;
using YomoneyApp.ViewModels;
using YomoneyApp.Views.Services;
using YomoneyApp.Models.Image;
using YomoneyApp.ViewModels.Countries;
using YomoneyApp.Models;
using System.Windows.Input;
using YomoneyApp.Popups;
using System.Threading;
using YomoneyApp.Utils;
using YomoneyApp.Views.Webview;
using System.IO;
using System.Net.Http.Headers;
using MvvmHelpers;
using YomoneyApp.Models.Province;
using YomoneyApp.Models.Town;
using YomoneyApp.Models.Nationality;
using YomoneyApp.Models.Supervisor;

namespace YomoneyApp
{
    public class AccountViewModel : ViewModelBase
    {
        string hostDomain = "https://192.168.100.172:45455";
        //string ProcessingCode = "350000";
       

        //public static int counter = 0;
        //public static int answerCounter = 0;

        //MapPageViewModel mapPageViewModel;
        //ServiceViewModel serviceViewModel;
        CountryPickerViewModel countryPickerViewModel;
        CancellationTokenSource cts;
        public ObservableRangeCollection<Province> Provinces { get; set; }
        public ObservableRangeCollection<Town> Towns { get; set; }
        public ObservableRangeCollection<Nationality> Nationalities { get; set; }
        public ObservableRangeCollection<Supervisor> Supervisors { get; set; }

        public AccountViewModel(Page page) : base(page)
        {
            Provinces = new ObservableRangeCollection<Province>();
            Towns = new ObservableRangeCollection<Town>();
            Nationalities = new ObservableRangeCollection<Nationality>();
            Supervisors = new ObservableRangeCollection<Supervisor>();

            Title = "Account";
            ShowPopupCommand = new Command(async _ => await ExecuteShowPopupCommand());
            CountrySelectedCommand = new Command(country => ExecuteCountrySelectedCommand(country as CountryModel));
        }

        #region Get Current Location
        private Command getCurrentGeolocationCommand;

        public Command GetCurrentGeolocationCommand
        {
            get
            {
                return getCurrentGeolocationCommand ??
                    (getCurrentGeolocationCommand = new Command(async () => await ExecuteGetCurrentGeolocationCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteGetCurrentGeolocationCommand()
        {
            //if (!IsBusy)
            //    return;

            IsBusy = true;
            Message = "Loading...";
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));

                cts = new CancellationTokenSource();

                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

                    var placemark = placemarks?.FirstOrDefault();

                    if (placemark != null)
                    {
                        var geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                        SelectedCountry =  await CountryUtils.GetCountryModelByName(placemark.CountryName);

                        IsBusy = false;

                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception

                await page.DisplayAlert("Error!", "The location feature is not supported on the device", "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception

                await page.DisplayAlert("Error!", "The location feature is not enabled on the device", "Ok");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception

                await page.DisplayAlert("Error!", "The location feature is not permitted on the device", "Ok");
            }
            catch (Exception ex)
            {
                // Unable to get location

                await page.DisplayAlert("Error!", "Unable to get the loaction on this device", "Ok");
            }
        }

        #endregion

        #region Show Alert
        private Command showMessageAlertCommand;

        public Command ShowMessageAlertCommand
        {
            get
            {
                return showMessageAlertCommand ??
                    (showMessageAlertCommand = new Command(async () => await ExecuteShowMessageAlertCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task ExecuteShowMessageAlertCommand()
        {
            await page.DisplayAlert("Success", "Active Country Updated Successfully", "Ok");
        }

        public void CallAlert()
        {
            page.DisplayAlert("Success", "Active Country Updated Successfully", "Ok");
        }
        #endregion

        #region login
        Command loginCommand;
        public Command LoginCommand
        {
            get
            {
                return loginCommand ??
                    (loginCommand = new Command(async () => await ExecuteLoginCommand(), () => { return !IsBusy; }));
            }
        }
        async Task ExecuteLoginCommand()
        {
            if (IsBusy)
            {
                return;
            }

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Phone Number Error!", "Please enter your mobile number", "OK");
                return;
            }
            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Phone Number Error!", "Please enter a valid mobile number", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Password2))
            {
                await page.DisplayAlert("Password Error!", "Please enter your password", "OK");
                return;
            }

            Password = Password2;

            Message = "Logging In...";
            IsBusy = true;
            loginCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = ActualPhoneNumber + ":" + Password;
                trn.MTI = "0100";
                trn.ProcessingCode = "200000";

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/Transaction/?{0}", myContent);

                if (transactionResponse.ResponseCode == "SignedIn")
                {
                    AccessSettings ac = new Services.AccessSettings();
                    string resp = "";

                    try
                    {
                        if (IsRememberMe) // If Checkbox is Checked
                        {
                            resp = ac.SaveCredentials(ActualPhoneNumber, Password, "RememberMe").Result;
                        }
                        else
                        {
                            resp = ac.SaveCredentials(ActualPhoneNumber, Password, "").Result;
                        }
                    }
                    catch
                    {
                        App.MyLogins = ActualPhoneNumber;
                        App.AuthToken = Password;
                        App.RememberMe = "";

                        resp = "00000";
                    }

                    if (resp == "00000")
                    {
                        await page.Navigation.PushAsync(new HomePage());
                    }
                    else
                    {
                        await page.DisplayAlert("Login Error!", "Your device is not compatable", "OK");
                    }
                }
                else
                {
                    ResponseDescription = "Invalid Username or Password";

                    await page.DisplayAlert("Login Error!", "Either your phone number or password is wrong", "OK");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "An error occurred while sending the request to the server")
                {
                    await page.DisplayAlert("Login Error", "There has been an error on trying to login. Please, go ahead and try again", "OK");
                }
                else
                {
                    await page.DisplayAlert("Login Error", ex.Message, "OK");
                }
            }
            finally
            {
                IsBusy = false;
                loginCommand?.ChangeCanExecute();
            }

            //await page.Navigation.PopAsync();

        }
        #endregion

        #region join
        Command joinCommand;
        public Command JoinCommand
        {
            get
            {
                return joinCommand ??
                    (joinCommand = new Command(async () => await ExecuteJoinCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteJoinCommand()
        {
            if (IsBusy)
                return;

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrEmpty(Name))
            {
                await page.DisplayAlert("Username Error!", "Please enter your username", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Firstname))
            {
                await page.DisplayAlert("First Name Error!", "Please enter your First Name", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Surname))
            {
                await page.DisplayAlert("Last Name Error!", "Please enter your Last Name", "OK");
                return;
            }

            //string[] nam = ContactName.Split(' ');

            //if (nam.Length < 2 || nam[0].Length < 3 || nam[1].Length < 3)
            //{
            //    await page.DisplayAlert("Full Name Error!", "Please enter your Fullname and Surname no initials", "OK");
            //    return;
            //}

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Phone Number Error!", "Please enter your phone number", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Gender))
            {
                await page.DisplayAlert("Gender!", "Please select your gender", "OK");
                return;
            }

            var formattedDate = Date.ToString("MM/dd/yyyy");

            if (string.IsNullOrEmpty(formattedDate))
            {
                await page.DisplayAlert("Date Error!", "Please enter your date of birth", "OK");
                return;
            }

            var ageInYears = GetDifferenceInYears(Date, DateTime.Today);

            if (ageInYears < 12)
            {
                await page.DisplayAlert("Age Error!", "You are too young to be using this app.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Password2))
            {
                await page.DisplayAlert("Password Error!", "Please enter a password", "OK");
                return;
            }

            Password = Password2;

            var isPasswordValid = ValidatePassword(Password);

            if (!isPasswordValid)
            {
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                await page.DisplayAlert("Confirm Password Error!", "Please confirm password", "OK");
                return;
            }

            if (confirmPassword != Password)
            {
                await page.DisplayAlert("Password Match Error!", "Confirmation password not matching password.", "OK");
                return;
            }

            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Phone Number Error!", "Please enter a valid mobile number! The length of the phone number you entered is not allowed.", "OK");
                return;
            }

            Message = "Creating Account...";
            IsBusy = true;
            joinCommand?.ChangeCanExecute();

            try
            {
                ContactName = Firstname + " " + Surname;

                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Name + "_" + ContactName + "_" + "NA" + "_" + ActualPhoneNumber + "_" + "NA" + "_" + Password + "_" + formattedDate + "_" + Gender;

                Name = null;
                ContactName = null;
                Date = DateTime.Now.Date;
                Password = null;
                ConfirmPassword = null;

                //page.Navigation.PopAsync();
                await page.Navigation.PushAsync(new VerificationPage(ActualPhoneNumber));

                //ActualPhoneNumber = null;
                //PhoneNumber = null;
                //IsBusy = false;

                MessagingCenter.Subscribe<string, string>("VerificationRequest", "VerifyMsg", async (sender, arg) =>
                {
                    if (arg == "Verified")
                    {
                        string Body = "";

                        Body += "Narrative=" + trn.Narrative;

                        HttpClient client = new HttpClient();

                        var myContent = Body;

                        WebService webService = new WebService();

                        var transactionResponse = await webService.GetResponse("/Mobile/Create/?{0}", myContent);

                        if (transactionResponse.ResponseCode == "00000")
                        {
                            try
                            {
                                AccessSettings ac = new Services.AccessSettings();

                                App.MyLogins = ActualPhoneNumber;
                                App.AuthToken = Password;
                                App.RememberMe = "";

                                //MenuItem mn = new MenuItem();

                                try
                                {
                                    var resp = ac.SaveCredentials(ActualPhoneNumber, Password, "").Result;

                                    if (!string.IsNullOrEmpty(transactionResponse.Note))
                                    {
                                        await page.DisplayAlert("Success!", "Your INGOMA account has been created successfully! Remember your credentials for logging in the next time", "OK");

                                        //MenuItem menuItem = new MenuItem();

                                        //menuItem.Id = "1";
                                        //menuItem.Image = "https://www.yomoneyservice.com/Content/Logos/ZAMTEL/zamtel.png";
                                        //menuItem.Title = "ZAMTEL";
                                        //menuItem.Note = "BANKING";
                                        //menuItem.TransactionType = 12;
                                        //menuItem.SupplierId = "5-0001-0001052";
                                        ////menuItem.date = "0001-01-01T00:00:00";

                                        //await page.Navigation.PushAsync(new ProviderServices(menuItem));

                                        await page.Navigation.PushAsync(new SignIn());

                                        #region Normal SignUp Process
                                        //switch (response.Note.ToUpper().Trim())
                                        //{
                                        //    case "ACTIVE":

                                        //        await page.DisplayAlert("Account Creation", "Account Created Successfully!", "OK");

                                        //        await page.Navigation.PushAsync(new AddEmailAddress(phone));

                                        //        break;

                                        //    default:
                                        //        await page.DisplayAlert("Account Creation Error", "There is something wrong on creating your account. Contact Customer Support", "OK");

                                        //        await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
                                        //        break;
                                        //}
                                        #endregion
                                    }
                                    else
                                    {
                                        await page.DisplayAlert("Account Creation Error!", "There is something wrong on creating your account. Contact Customer Support", "OK");

                                        string action = await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

                                        if (!string.IsNullOrEmpty(action))
                                        {
                                            try
                                            {
                                                Device.OpenUri(new Uri("https://wa.link/fehh38"));
                                            }
                                            catch (Exception ex)
                                            {
                                                await page.DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    await page.DisplayAlert("Account Creation Error", "An error has occured whilst saving the transaction. Contact Customer Support", "OK");

                                    string action = await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

                                    if (!string.IsNullOrEmpty(action))
                                    {
                                        try
                                        {
                                            Device.OpenUri(new Uri("https://wa.link/fehh38"));
                                        }
                                        catch (Exception ex)
                                        {
                                            await page.DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                await page.DisplayAlert("Account Creation Error", "An error has occured. Check the application permissions and allow data storage or Contact Customer Support", "OK");

                                string action = await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

                                if (!string.IsNullOrEmpty(action))
                                {
                                    try
                                    {
                                        Device.OpenUri(new Uri("https://wa.link/fehh38"));
                                    }
                                    catch (Exception ex)
                                    {
                                        await page.DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                                    }
                                }
                            }
                        }
                        else
                        {
                            await page.DisplayAlert("Error", transactionResponse.Description, "OK");

                            string action = await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

                            if (!string.IsNullOrEmpty(action))
                            {
                                try
                                {
                                    Device.OpenUri(new Uri("https://wa.link/fehh38"));
                                }
                                catch (Exception ex)
                                {
                                    await page.DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Please enter a valid verification code";

                        await page.DisplayAlert("Account Creation Error", message, "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Join Error", "Unable to create your account, please check your internet connection and try again", "OK");
            }
            finally
            {
                IsBusy = false;
                joinCommand?.ChangeCanExecute();
            }

            // await page.Navigation.PopAsync();

        }
        #endregion

        #region First Next Command

        Command firstNextCommand;
        public Command FirstNextCommand
        {
            get
            {
                return firstNextCommand ??
                    (firstNextCommand = new Command(async () => await ExecuteFirstNextCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteFirstNextCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(FormFirstname))
            {
                await page.DisplayAlert("Enter Firstname", "Please enter your firstname", "OK");
                return;
            }           
            else if (string.IsNullOrWhiteSpace(FormLastname))
            {
                await page.DisplayAlert("Enter Lastname", "Please enter your lastname", "OK");
                return;
            }
            else if (string.IsNullOrWhiteSpace(FormDeviceOwnership))
            {
                await page.DisplayAlert("Enter Username", "Please specify device ownership", "OK");
                return;
            }            
            else if (string.IsNullOrWhiteSpace(FormMobileNumber))
            {
                await page.DisplayAlert("Enter Mobile Number", "Please enter your mobile number", "OK");
                return;
            }           
            else if (FormMobileNumber == FormAlternativeMobileNumber)
            {
                await page.DisplayAlert("Mobile Numbers Error", "Mobile number and Alternative Mobile Number have to be different", "OK");
                return;
            }
            else if (string.IsNullOrWhiteSpace(FormGender))
            {
                await page.DisplayAlert("Select Gender", "Please select gender", "OK");
                return;
            }
            else if (string.IsNullOrWhiteSpace(FormPassword))
            {
                await page.DisplayAlert("Enter Password", "Please enter password", "OK");
                return;
            }
            else if (FormPassword != ConfirmPassword)
            {
                await page.DisplayAlert("Passwords Match", "Password and Confirm Password field do not match", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            firstNextCommand?.ChangeCanExecute();

            try
            {
                DeviceOwnership = FormDeviceOwnership;
                Firstname = FormFirstname;
                Middlename = FormMiddlename;
                Lastname = FormLastname;
                MobileNumber = FormMobileNumber;
                AlternativeMobileNumber = FormAlternativeMobileNumber;
                Gender = FormGender;
                Password = FormPassword;

                await page.Navigation.PushAsync(new AgentRegistrationTwo());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Processing error. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyEmailOTPCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region Second Next Command

        Command secondNextCommand;
        public Command SecondNextCommand
        {
            get
            {
                return secondNextCommand ??
                    (secondNextCommand = new Command(async () => await ExecuteSecondNextCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSecondNextCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(FormArea))
            {
                await page.DisplayAlert("Enter Area", "Please enter your area", "OK");
                return;
            }
            else if (string.IsNullOrWhiteSpace(FormIdNumber))
            {
                await page.DisplayAlert("Enter NIC Number", "Please enter your National Id Number (NIC)", "OK");
                return;
            }
            else if (ProvinceId < 1)
            {
                await page.DisplayAlert("Select Province", "Please select your province", "OK");
                return;
            }
            else if (TownId < 1)
            {
                await page.DisplayAlert("Select Town", "Please select your town", "OK");
                return;
            }
            else if (SupervisorId < 1)
            {
                await page.DisplayAlert("Select Supervisor", "Please select your supervisor", "OK");
                return;
            }
            else if (NationalityId < 1)
            {
                await page.DisplayAlert("Select Nationality", "Please select your nationality", "OK");
                return;
            }
            
            Message = "Processing...";
            IsBusy = true;
            secondNextCommand?.ChangeCanExecute();

            try
            {
                Area = FormArea;
                AgentCode = FormAgentCode;
                IdNumber = FormIdNumber;               

                await page.Navigation.PushAsync(new AgentRegistrationThree());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Processing error. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
                secondNextCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region Load Provinces, Towns, Nationalities

        Command loadProvincesCommand;
        public Command LoadProvinces
        {
            get
            {
                return loadProvincesCommand ??
                    (loadProvincesCommand = new Command(async () => await ExecuteLoadProvincesCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task<IEnumerable<Province>> ExecuteLoadProvincesCommand()
        {
            if (IsBusy)
                return new List<Province>();

            Message = "Loading...";
            IsBusy = true;
            loadProvincesCommand?.ChangeCanExecute();

            try
            {
                string uri = hostDomain + "/api/provinces";

                WebService webService = new WebService();

                var provinces = await WebService.GetProvincesAsync(uri);

                if (provinces != null)
                {
                    Provinces.Clear();
                    Provinces.AddRange(provinces);

                    return Provinces;
                }
                else
                {
                    await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                    return Provinces;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                return Provinces;
            }
            finally
            {
                IsBusy = false;
                loadProvincesCommand?.ChangeCanExecute();
            }
        }
                
        public async Task<IEnumerable<Town>> ExecuteLoadTownsCommand(string province)
        {
            if (IsBusy)
                return new List<Town>();

            Message = "Loading...";
            IsBusy = true;
            
            try
            {
                string uri = hostDomain + "/api/towns";

                WebService webService = new WebService();

                var actualProvince = Provinces.Where(x => x.Name.ToLower().Trim() == province.ToLower().Trim()).FirstOrDefault();
                FormProvinceId = actualProvince.Id;
                ProvinceId = FormProvinceId;

                var towns = await WebService.GetTownsAsync(uri);

                var provincialTowns = towns.Where(x => x.ProvinceId == actualProvince.Id).ToList();

                if (provincialTowns != null)
                {
                    Towns.Clear();
                    Towns.AddRange(provincialTowns);

                    return Towns;
                }
                else
                {
                    await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                    return Towns;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                return Towns;
            }
            finally
            {
                IsBusy = false;                
            }
        }

        public async Task<IEnumerable<Supervisor>> ExecuteLoadSupervisorsCommand(string province)
        {
            if (IsBusy)
                return new List<Supervisor>();

            Message = "Loading...";
            IsBusy = true;

            try
            {
                string uri = hostDomain + "/api/supervisors";

                WebService webService = new WebService();

                var actualProvince = Provinces.Where(x => x.Name.ToLower().Trim() == province.ToLower().Trim()).FirstOrDefault();

                if (ProvinceId != actualProvince.Id)
                {
                    FormProvinceId = actualProvince.Id;
                    ProvinceId = FormProvinceId;
                }

                var supervisors = await WebService.GetSupervisorsAsync(uri);

                var provincialSupervisors = supervisors.Where(x => x.ProvinceId == actualProvince.Id).ToList();

                if (provincialSupervisors != null)
                {
                    Supervisors.Clear();
                    Supervisors.AddRange(provincialSupervisors);

                    return Supervisors;
                }
                else
                {
                    await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                    return Supervisors;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                return Supervisors;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteStoreTownIdCommand(string town)
        {           
            Message = "Processing...";
            IsBusy = true;

            try
            {
                var currentTown = Towns.Where(x => x.Name.ToLower().Trim() == town.ToLower().Trim()).FirstOrDefault();

                if (TownId < 1)
                {
                    TownId = currentTown.Id;
                }
                else
                {
                    TownId = currentTown.Id;
                }
             
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Processing error. Please try again", "OK");                
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteStoreSupervisorCommand(string supervisor)
        {
            Message = "Processing...";
            IsBusy = true;

            try
            {
                var splittedNames = supervisor.Split(' ');

                var currentSupervisor = Supervisors.Where(x => x.Firstname.ToLower().Trim() == splittedNames[0].ToLower().Trim() && x.Lastname.ToLower().Trim() == splittedNames[1].ToLower().Trim()).FirstOrDefault();

                if (SupervisorId < 1)
                {
                    SupervisorId = currentSupervisor.Id;
                }
                else
                {
                    SupervisorId = currentSupervisor.Id;
                }

            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Processing error. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteStoreNationalityCommand(string nationality)
        {
            Message = "Processing...";
            IsBusy = true;

            try
            {
                var currentNationality = Nationalities.Where(x => x.Name.ToLower().Trim() == nationality.ToLower().Trim()).FirstOrDefault();

                if (NationalityId < 1)
                {
                    NationalityId = currentNationality.Id;
                }
                else
                {
                    NationalityId = currentNationality.Id;
                }

            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Processing error. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        Command loadNationalitiesCommand;
        public Command LoadNationalities
        {
            get
            {
                return loadNationalitiesCommand ??
                    (loadNationalitiesCommand = new Command(async () => await ExecuteLoadNationalitiesCommand(), () => { return !IsBusy; }));
            }
        }

        public async Task<IEnumerable<Nationality>> ExecuteLoadNationalitiesCommand()
        {
            if (IsBusy)
                return new List<Nationality>();

            Message = "Loading...";
            IsBusy = true;
            loadNationalitiesCommand?.ChangeCanExecute();

            try
            {
                string uri = hostDomain + "/api/nationalities";

                WebService webService = new WebService();

                var nationalities = await WebService.GetNationalitiesAsync(uri);

                if (nationalities != null)
                {
                    Nationalities.Clear();
                    Nationalities.AddRange(nationalities);

                    return Nationalities;
                }
                else
                {
                    await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                    return Nationalities;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "Loading error. Please try again", "OK");

                return Nationalities;
            }
            finally
            {
                IsBusy = false;
                loadNationalitiesCommand?.ChangeCanExecute();
            }
        }

        #endregion

        int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year - 1) +
                (((endDate.Month > startDate.Month) ||
                ((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);
        }       
              
        #region EmailOption

        Command submitEmailCommand;
        public Command SubmitEmailCommand
        {
            get
            {
                return submitEmailCommand ??
                    (submitEmailCommand = new Command(async () => await ExecuteSubmitEmailCommand(), () => { return !IsBusy; }));
            }
        }

        [Obsolete]
        async Task ExecuteSubmitEmailCommand()
        {
            if (IsBusy)
                return;

            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            if (string.IsNullOrWhiteSpace(Email))
            {
                await page.DisplayAlert("Enter Email Address", "Please enter your email address.", "OK");
                return;
            }
            else if (!Regex.IsMatch(Email, pattern))
            {
                await page.DisplayAlert("Enter Valid Email Address", "Please enter a valid email address.", "OK");
                return;
            }

            Message = "Submitting Email...";
            IsBusy = true;
            submitEmailCommand?.ChangeCanExecute();

            try
            {
                AccessSettings accessSettings = new AccessSettings();
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Email + "_" + accessSettings.UserName;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(hostDomain + "/Mobile/EmailLookup/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        try
                        {
                            await page.DisplayAlert("Email Validation", "Email Validated Successfully", "OK");

                            await page.Navigation.PushAsync(new PasswordReset(accessSettings.UserName, Email));
                        }
                        catch (Exception e)
                        {
                            await page.DisplayAlert("Error", e.Message, "OK");
                        }
                    }
                    else
                    {
                        await page.DisplayAlert("Error", response.Description + ". You can contact Customer Support for Assistance", "OK");
                        string action = await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +27 79 190 3850");

                        if (!string.IsNullOrEmpty(action))
                        {
                            try
                            {
                                Device.OpenUri(new Uri("https://wa.link/fehh38"));
                            }
                            catch (Exception ex)
                            {
                                await page.DisplayAlert("Not Installed", "Whatsapp Not Installed", "ok");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Email Error", "There has been an error in adding your email address.", "OK");
            }
            finally
            {
                IsBusy = false;
                submitEmailCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region Forgot
        Command forgotCommand;
        public Command ForgotCommand
        {
            get
            {
                return forgotCommand ??
                    (forgotCommand = new Command(async () => await ExecuteForgotCommand(), () => { return !IsBusy; }));
            }
        }
        async Task ExecuteForgotCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await page.DisplayAlert("Enter Mobile Number", "Please enter a valid mobile number. e.g 263775555000", "OK");
                return;
            }
            else if (PhoneNumber.Length != 12)
            {
                await page.DisplayAlert("Enter Mobile Number", "Please enter a valid mobile number. e.g 263775555000", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            ForgotCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();
                Password = "NA";
                trn.CustomerAccount = phone + ":" + Password;
                trn.MTI = "0100";
                trn.Narrative = phone + "_" + Password;
                string Body = "";
                Body += "Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&MTI=0100";
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(hostDomain + "/Mobile/ForgotPassword/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                    if (response.Note == "VerificationCode" && response.ResponseCode == "00000")
                    {

                        await page.Navigation.PushAsync(new VerificationPage(phone));
                    }
                    else
                    {
                        ResponseDescription = response.ResponseCode;
                    }

                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", "Some of the information you entered in incorrect", "OK");
            }
            finally
            {
                IsBusy = false;
                forgotCommand?.ChangeCanExecute();
            }

            //await page.Navigation.PopAsync();

        }
        #endregion

        #region VerifyQuestionCommand

        Command verifyQuestionCommand;
        public Command VerifyQuestionCommand
        {
            get
            {
                return verifyQuestionCommand ??
                    (verifyQuestionCommand = new Command(async () => await ExecuteVerifyQuestionCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyQuestionCommand()
        {
            if (IsBusy)
                return;

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter a your mobile number! Phone Number field cannot be empty.", "OK");
                return;
            }
            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Error!", "Please enter a valid mobile number!", "OK");
                return;
            }

            Message = "Submitting Phone Number...";
            IsBusy = true;
            verifyQuestionCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = ActualPhoneNumber;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(hostDomain + "/Mobile/PhoneNumberVerification/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        try
                        {
                            await page.Navigation.PushAsync(new QuestionOTPPage(ActualPhoneNumber));
                        }
                        catch (Exception e)
                        {
                            await page.DisplayAlert("Error", e.Message, "OK");
                        }
                    }
                    else
                    {
                        await page.DisplayAlert("Error", response.Description, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Email Error", "Unable to add your email address, please check your internet connection and try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyQuestionCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region VerifyEmail
        Command verifyEmailCommand;
        public Command VerifyEmailCommand
        {
            get
            {
                return verifyEmailCommand ??
                    (verifyEmailCommand = new Command(async () => await ExecuteVerifyEmailCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyEmailCommand()
        {
            if (IsBusy)
                return;

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter a your mobile number! Phone Number field cannot be empty.", "OK");
                return;
            }
            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Error!", "Please enter a valid mobile number!", "OK");
                return;
            }

            Message = "Submitting Phone Number...";
            IsBusy = true;
            verifyEmailCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = ActualPhoneNumber;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(hostDomain + "/Mobile/PhoneNumberVerification/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        try
                        {
                            await page.Navigation.PushAsync(new EmailOTPPage(ActualPhoneNumber));
                        }
                        catch (Exception e)
                        {
                            await page.DisplayAlert("Error", e.Message, "OK");
                        }
                    }
                    else
                    {
                        await page.DisplayAlert("Error", response.Description, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Email Error", "Unable to add your email address, please check your internet connection and try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyEmailCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region VerifyPhoneNumber
        Command verifyPhoneNumberCommand;
        public Command VerifyPhoneNumberCommand
        {
            get
            {
                return verifyPhoneNumberCommand ??
                    (verifyPhoneNumberCommand = new Command(async () => await ExecuteVerifyPhoneNumberCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyPhoneNumberCommand()
        {
            if (IsBusy)
                return;

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter a your mobile number! Phone Number field cannot be empty.", "OK");
                return;
            }
            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Error!", "Please enter a valid mobile number!", "OK");
                return;
            }

            Message = "Submitting Phone Number...";
            IsBusy = true;
            verifyPhoneNumberCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = ActualPhoneNumber;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/PhoneNumberVerification/?{0}", myContent);

                if (transactionResponse.ResponseCode == "00000")
                {
                    try
                    {
                        await page.Navigation.PushAsync(new Views.Login.OTPPage(ActualPhoneNumber));
                    }
                    catch (Exception e)
                    {
                        await page.DisplayAlert("Error!", "There has been an error in verifying your mobile number. Please, go ahead and try again", "OK");
                    }
                }
                else
                {
                    await page.DisplayAlert("Error", transactionResponse.Description, "OK");
                }

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error", "There has been an error in verifying your mobile number. Please, go ahead and try again", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyPhoneNumberCommand?.ChangeCanExecute();
            }
        }

        #endregion

        #region Verify

        #region VerifyFormOTPCommand

        Command verifyFormOTPCommand;
        public Command VerifyFormOTPCommand
        {
            get
            {
                return verifyFormOTPCommand ??
                    (verifyFormOTPCommand = new Command(async () => await ExecuteVerifyFormOTPCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyFormOTPCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Password2))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Password = Password2;

            Message = "Processing...";
            IsBusy = true;
            verifyFormOTPCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                AccessSettings acnt = new AccessSettings();
                string pass = acnt.Password;
                string uname = acnt.UserName;

                if (string.IsNullOrWhiteSpace(pass))
                {
                    pass = AccountViewModel.Password;
                }

                if (string.IsNullOrWhiteSpace(uname))
                {
                    uname = AccountViewModel.ActualPhoneNumber;
                }

                phone = uname;

                trn.CustomerAccount = phone + ":" + Password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = Password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + Password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/Transaction/?{0}", myContent);

                MenuItem mn = new MenuItem();

                if (transactionResponse.ResponseCode == "00000")
                {
                    // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                    await page.DisplayAlert("Success", "OTP Successfully Verified!", "OK");

                    // HomeViewModel.fileUpload.FormId = HomeViewModel.fileUpload.FormId + 1;

                    if (!string.IsNullOrEmpty(HomeViewModel.fileUpload.RecordId))
                    {
                        //string weblink = "https://www.yomoneyservice.com/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&ServiceId=" +
                        //HomeViewModel.fileUpload.ServiceId + "&ActionId=" + HomeViewModel.fileUpload.ActionId +
                        //"&Customer=" + HomeViewModel.fileUpload.PhoneNumber + "&RecordId=" + HomeViewModel.fileUpload.RecordId +
                        //"&FormNumber=" + HomeViewModel.fileUpload.FormId + "&CallType=FirstTime";

                        try
                        {
                            AccessSettings accessSettings = new AccessSettings();
                            string password = acnt.Password;
                            string username = acnt.UserName;

                            FileUpload fileUpload = new FileUpload();

                            fileUpload.Name = "true";
                            //fileUpload.Type = "";
                            fileUpload.PhoneNumber = uname;
                            //fileUpload.Image = "";
                            fileUpload.Purpose = "FIELD";
                            fileUpload.ServiceId = HomeViewModel.fileUpload.ServiceId;
                            fileUpload.ActionId = HomeViewModel.fileUpload.ActionId;
                            fileUpload.SupplierId = HomeViewModel.fileUpload.SupplierId;
                            fileUpload.FormId = HomeViewModel.fileUpload.FormId;
                            fileUpload.FieldId = HomeViewModel.fileUpload.FieldId;
                            fileUpload.RecordId = HomeViewModel.fileUpload.RecordId;

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
                                        //await page.DisplayAlert("File Upload", "File scanned and saved successfully", "OK");
                                        IsBusy = false;

                                        Device.BeginInvokeOnMainThread(async () =>
                                        {
                                            //viewModel.IsBusy = false;
                                            //FileImage.Source = null;                                               

                                            page.Navigation.PopAsync();

                                            await page.Navigation.PushAsync(new WebviewHyubridConfirm("https://www.yomoneyservice.com" + serverresult, "OTP Verification", false, null));
                                        });
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(async () =>
                                        {
                                            await page.DisplayAlert("Error!", "There was an error in verifying you at the server. Could you start again the process.", "OK");
                                            IsBusy = false;

                                        });

                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);

                            await page.DisplayAlert("Error!", "There was an error in verifying you at the server. Could you start again the process.", "OK");
                        }

                        //await page.Navigation.PushAsync(new WebviewHyubridConfirm(weblink, "OTP Verification", false, null, false));
                    }
                    else
                    {
                        string weblink = "https://www.yomoneyservice.com/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&ServiceId=" +
                       HomeViewModel.fileUpload.ServiceId + "&ActionId=" + HomeViewModel.fileUpload.ActionId +
                       "&Customer=" + HomeViewModel.fileUpload.PhoneNumber +
                       "&FormNumber=" + HomeViewModel.fileUpload.FormId + "&CallType=FirstTime";

                        await page.Navigation.PushAsync(new WebviewHyubridConfirm(weblink, "OTP Verification", false, null, false));
                    }
                }
                else if (transactionResponse.ResponseCode == "Error" || transactionResponse.ResponseCode == "00008")
                {
                    mn.Note = phone;
                    await page.DisplayAlert("Error", "Failed to verify your OTP. Please try again the process.", "OK");
                }
                else
                {
                    mn.Note = phone;
                    await page.DisplayAlert("OTP Verification", "Failed to verify your OTP. Please try again the process.", "OK");
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                verifyEmailOTPCommand?.ChangeCanExecute();
            }
        }

        #endregion        

        #region VerifyPhoneOTPCommand

        Command verifyPhoneOTPCommand;
        public Command VerifyPhoneOTPCommand
        {
            get
            {
                return verifyPhoneOTPCommand ??
                    (verifyPhoneOTPCommand = new Command(async () => await ExecuteVerifyPhoneOTPCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyPhoneOTPCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Password2))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Password = Password2;

            Message = "Processing...";
            IsBusy = true;
            verifyPhoneOTPCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + Password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = Password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + Password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/Transaction/?{0}", myContent);

                MenuItem mn = new MenuItem();

                if (transactionResponse.ResponseCode == "00000")
                {
                    AccessSettings accessSettings = new AccessSettings();

                    string userName = accessSettings.UserName;

                    if (string.IsNullOrEmpty(userName))
                    {
                        userName = PhoneNumber;
                    }

                    await page.Navigation.PushAsync(new PasswordReset(userName, ""));
                }
                else if (transactionResponse.ResponseCode == "Error" || transactionResponse.ResponseCode == "00008")
                {
                    mn.Note = phone;
                    await page.DisplayAlert("Error", "Failed to verify your OTP. Please try again the process.", "OK");
                }
                else
                {
                    mn.Note = phone;
                    await page.DisplayAlert("OTP Verification", "Failed to verify your OTP.Please try again the process.", "OK");
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", "There has been an error in verifying your OTP, please go ahead and repeat the process", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyPhoneOTPCommand?.ChangeCanExecute();
            }
        }

        #endregion

        Command verifyEmailOTPCommand;
        public Command VerifyEmailOTPCommand
        {
            get
            {
                return verifyEmailOTPCommand ??
                    (verifyEmailOTPCommand = new Command(async () => await ExecuteVerifyEmailOTPCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyEmailOTPCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            verifyEmailOTPCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + Password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = Password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + Password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(hostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        await page.Navigation.PushAsync(new EmailAddress(PhoneNumber));
                    }
                    else if (response.ResponseCode == "Error" || response.ResponseCode == "00008")
                    {
                        mn.Note = phone;
                        await page.DisplayAlert("Error", "Failed to verify your OTP. Please try again the process.", "OK");
                    }
                    else
                    {
                        mn.Note = phone;
                        await page.DisplayAlert("OTP Verification", "Failed to verify your OTP.Please try again the process.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                verifyEmailOTPCommand?.ChangeCanExecute();
            }
        }

        public async void GetVerificationAsync()
        {
            if (IsBusy)
                return;

            var showAlert = false;

            //string myinput = await PaymentCall(page.Navigation, "Payment");
            //MenuItem mn = new YomoneyApp.MenuItem();
            //mn.Amount = String.Format("{0:n}", Math.Round(decimal.Parse(budget), 2).ToString());
            //mn.Title = Category;

            // Message = "Processing request";
            #region process
            try
            {
                // if (ServiceOptions != null)
                //   ServiceOptions.Clear();
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "210000";
                trn.Note = "Supplier";
                trn.TerminalId = "ClientApp";
                trn.TransactionRef = Email;
                trn.Mpin = Password;
                trn.Narrative = "Verification";

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&OrderLines=" + trn.OrderLines;
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&Currency=" + trn.Currency;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Quantity=" + trn.Quantity;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;
                Body += "&TransactionType=" + trn.TransactionType;

                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(180);

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/Transaction/?{0}", myContent);

                if (transactionResponse.ResponseCode == "Success" || transactionResponse.ResponseCode == "00000")
                {

                }
                else
                {
                    //IsConfirm = false;
                    // Retry = true;
                    await page.DisplayAlert("OTP Error", "Sorry, we are not able to send you an OTP moment, please try again later." + " Please try again ", "OK");
                }
            }
            catch (Exception ex)
            {
                showAlert = true;
            }
            finally
            {
                IsBusy = false;
                //GetTokenCommand.ChangeCanExecute();
            }
            #endregion

            //Message = "";
            if (showAlert)
                await page.DisplayAlert("Transaction Error", "The service timed out please retry to get response", "OK");

        }

        Command verifyCommand;
        public Command VerifyCommand
        {
            get
            {
                return verifyCommand ??
                    (verifyCommand = new Command(async () => await ExecuteVerifyCommand(), () => { return !IsBusy; }));
            }
        }
        async Task ExecuteVerifyCommand()
        {
            if (IsBusy)
                return;
            if (string.IsNullOrWhiteSpace(Password2))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            VerifyCommand?.ChangeCanExecute();

            // Password = Password2;

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + Password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = Password2;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + Password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/Transaction/?{0}", myContent);

                MenuItem mn = new MenuItem();

                if (transactionResponse.ResponseCode == "00000")
                {
                    // mn.Description = "Your new password will be sent to your email address which starts and ends as shown bellow";
                    //mn.Title = response.Narrative;
                    //mn.Note = phone;
                    MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                    //await page.Navigation.PushAsync(new AddEmailAddress(mn));
                }
                else if (transactionResponse.ResponseCode == "Error" || transactionResponse.ResponseCode == "00008")
                {
                    //mn.Description = "You need a valid email address for password reset please contact customer service";
                    mn.Note = phone;
                    await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
                }
                else
                {
                    //mn.Description = "Please enter an email address for your account where you new password will be sent";
                    mn.Note = phone;
                    await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Oh Oooh :(", "Unable to verify code .", "OK");
            }
            finally
            {
                IsBusy = false;
                verifyCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region Reset
        Command resetCommand;
        public Command ResetCommand
        {
            get
            {
                return resetCommand ??
                    (resetCommand = new Command(async () => await ExecuteResetCommand(), () => { return !IsBusy; }));
            }
        }


        async Task ExecuteResetCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Password2))
            {
                await page.DisplayAlert("Enter Password", "Please enter a password.", "OK");
                return;
            }

            Password = Password2;

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                await page.DisplayAlert("Confirm Password", "Please confirm password.", "OK");
                return;
            }

            if (confirmPassword != Password)
            {
                await page.DisplayAlert("Confirm Password", "Confirmation password not matching password.", "OK");
                return;
            }

            var isPasswordValid = ValidatePassword(Password);

            if (!isPasswordValid)
            {
                return;
            }

            Message = "Resetting Password...";
            IsBusy = true;
            ResetCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                if (string.IsNullOrEmpty(email))
                {
                    email = "na";
                }

                AccessSettings access = new AccessSettings();

                string uname = string.Empty;

                if (string.IsNullOrEmpty(access.UserName))
                {
                    uname = AccountViewModel.ActualPhoneNumber;
                }
                else
                {
                    uname = access.UserName;
                }

                trn.Narrative = uname + "_" + email + "_" + Password;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                WebService webService = new WebService();

                var transactionResponse = await webService.GetResponse("/Mobile/ResetPassword/?{0}", myContent);

                MenuItem mn = new MenuItem();

                if (transactionResponse.ResponseCode == "00000")
                {
                    AccessSettings ac = new Services.AccessSettings();

                    App.MyLogins = PhoneNumber;
                    App.AuthToken = Password;
                    App.RememberMe = "";

                    var resp = ac.SaveCredentials(PhoneNumber, Password, "");

                    await page.DisplayAlert("Password Reset", "Password Reset Successful!", "OK");

                    IsBusy = false;

                    await page.Navigation.PushAsync(new SignIn());
                }
                else
                {
                    ResponseDescription = transactionResponse.Description;

                    await page.DisplayAlert("Error!", ResponseDescription, "OK");

                    await page.Navigation.PushAsync(new SignIn());
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Oh Oooh :(", "Unable to reset password. Check you internet connection and  try again", "OK");

                await page.Navigation.PushAsync(new SignIn());
            }
            finally
            {
                IsBusy = false;
                resetCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region Accept Customer Phone Number
        Command acceptCustomerPhoneNumber;
        public Command AcceptCustomerPhoneNumber
        {
            get
            {
                return acceptCustomerPhoneNumber ??
                    (acceptCustomerPhoneNumber = new Command(async () => await ExecuteAcceptPhoneNumberCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteAcceptPhoneNumberCommand()
        {
            if (IsBusy)
                return;

            ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter a your mobile number! Phone Number field cannot be empty.", "OK");
                return;
            }
            else if (ActualPhoneNumber.Length > 15)
            {
                await page.DisplayAlert("Error!", "Please enter a valid mobile number!", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(ActualPhoneNumber))
            {
                await page.DisplayAlert("Error!", "Please enter your password! Password field cannot be empty.", "OK");
                return;
            }

            Message = "Submitting Phone Number...";

            IsBusy = true;
            acceptCustomerPhoneNumber?.ChangeCanExecute();

            try
            {
                ActualPhoneNumber = ActualPhoneNumber;

                // await page.Navigation.PushAsync(new OtpLogin(PhoneNumber));
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Error!", "There has been an error in capturing your phone number. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
                acceptCustomerPhoneNumber?.ChangeCanExecute();
            }
        }
        #endregion

        #region Otp Login
        Command otpLogin;
        public Command OtpLogin
        {
            get
            {
                return otpLogin ??
                    (otpLogin = new Command(async () => await ExecuteOtpLoginCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteOtpLoginCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter OTP", "Please enter your One-Time Password.", "OK");
                return;
            }

            Message = "Verifying One-Time Password...";
            IsBusy = true;
            otpLogin?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = PhoneNumber + ":" + Password;
                trn.Mpin = Password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = PhoneNumber + "_" + Password;

                string Body = "";

                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(hostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        await page.Navigation.PushAsync(new AddEmailAddress(PhoneNumber));
                    }
                    else if (response.ResponseCode == "Error" || response.ResponseCode == "00008")
                    {
                        //mn.Description = "You need a valid email address for password reset please contact customer service";
                        mn.Note = phone;
                        await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
                    }
                    else
                    {
                        //mn.Description = "Please enter an email address for your account where you new password will be sent";
                        mn.Note = phone;
                        await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await page.DisplayAlert("Error!", "There has been an error in submitting your OTP. Please try again", "OK");
            }
            finally
            {
                IsBusy = false;
                otpLogin?.ChangeCanExecute();
            }
        }
        #endregion

        #region CountryPicker Functionality

        #region Commands

        public ICommand ShowPopupCommand { get; }
        public ICommand CountrySelectedCommand { get; }

        #endregion Commands

        #region Private Methods

        public Task ExecuteShowPopupCommand()
        {
            var popup = new ChooseCountryPopup(SelectedCountry)
            {
                CountrySelectedCommand = CountrySelectedCommand
            };

            return Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
        }

        public void ExecuteCountrySelectedCommand(CountryModel country)
        {
            SelectedCountry = country;
        }

        #endregion Private Methods

        #endregion

        #region model
        CountryModel selectedCountry;

        public CountryModel SelectedCountry
        {
            get { return selectedCountry; }
            set { SetProperty(ref selectedCountry, value); }
        }

        public UserAccount UserObj { get; set; }
        bool requiresCall = false;

        public bool RequiresCall
        {
            get { return requiresCall; }
            set { SetProperty(ref requiresCall, value); }
        }

        bool isRememberMe = false;
        public bool IsRememberMe
        {
            get { return isRememberMe; }
            set { SetProperty(ref isRememberMe, value); }
        }
                
        string securityQuestion = string.Empty;
        public string SecurityQuestion
        {
            get { return securityQuestion; }
            set { SetProperty(ref securityQuestion, value); }
        }

        List<SecurityQuestions> securityQuestions;
        public List<SecurityQuestions> SecurityQuestions
        {
            get { return securityQuestions; }
            set { SetProperty(ref securityQuestions, value); }
        }

        string responseDescription = string.Empty;
        public string ResponseDescription
        {
            get { return responseDescription; }
            set { SetProperty(ref responseDescription, value); }
        }

        string phone = string.Empty;
        public string PhoneNumber
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        //string actualPhoneNumber = string.Empty;
        //public static string ActualPhoneNumber
        //{
        //    get { return actualPhoneNumber; }
        //    set { SetProperty(ref actualPhoneNumber, value); }
        //}

        public static string ActualPhoneNumber { get; set; }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        //string firstname = string.Empty;
        //public string Firstname
        //{
        //    get { return firstname; }
        //    set { SetProperty(ref firstname, value); }
        //}

        string surname = string.Empty;
        public string Surname
        {
            get { return surname; }
            set { SetProperty(ref surname, value); }
        }

        string password2 = string.Empty;
        public string Password2
        {
            get { return password2; }
            set { SetProperty(ref password2, value); }
        }

        string contactname = string.Empty;
        public string ContactName
        {
            get { return contactname; }
            set { SetProperty(ref contactname, value); }
        }

        string message = "Loading...";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        string answer = string.Empty;
        public string Answer
        {
            get { return answer; }
            set { SetProperty(ref answer, value); }
        }

        string email = string.Empty;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        //string password = string.Empty;
        //public string Password
        //{
        //    get { return password; }
        //    set { SetProperty(ref password, value); }
        //}
        
        string confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { SetProperty(ref confirmPassword, value); }
        }

        int serviceType = 4;
        public int ServiceType
        {
            get { return serviceType; }
            set
            {
                SetProperty(ref serviceType, value);
            }
        }

        int rating = 10;
        public int Rating
        {
            get { return rating; }
            set
            {
                SetProperty(ref rating, value);
            }
        }

        DateTime date = Today;
        public DateTime Date
        {
            get { return date; }
            set
            {
                SetProperty(ref date, value);
            }
        }

        public string StoreName { get; set; } = string.Empty;
        #endregion

        bool invalid = false;

        #region Zamtel Agent Model

        public static string Firstname { get; set; }        
        public static string Middlename { get; set; }
        public static string Lastname { get; set; }
        public static string DeviceOwnership { get; set; }
        public static int? SupervisorId { get; set; }
        public static string Password { get; set; }
        public static string Gender { get; set; }
        public static string Area { get; set; }
        public static int? TownId { get; set; }
        public static int? ProvinceId { get; set; }
        public static int? NationalityId { get; set; }
        public static string IdNumber { get; set; }
        public static string MobileNumber { get; set; }
        public static string AlternativeMobileNumber { get; set; }
        public static string AgentCode { get; set; }
        public static string PotrailUrl { get; set; }
        public static string NationalIdFrontUrl { get; set; }
        public static string NationalIdBackUrl { get; set; }
        public static string SignatureUrl { get; set; }
        public static string AgentContractFormUrl { get; set; }
        public static bool? IsVerified { get; set; }
        public static int? NextOfKinId { get; set; }       
        public static int? CreatedByUserId { get; set; }
        public static int? ModifiedByUserId { get; set; }
        public static string Username { get; set; }

        #endregion

        #region Zamtel Form Model

        string formFirstname = string.Empty;
        public string FormFirstname
        {
            get { return formFirstname; }
            set { SetProperty(ref formFirstname, value); }
        }

        string formMiddlename = string.Empty;
        public string FormMiddlename
        {
            get { return formMiddlename; }
            set { SetProperty(ref formMiddlename, value); }
        }

        string formLastname = string.Empty;
        public string FormLastname
        {
            get { return formLastname; }
            set { SetProperty(ref formLastname, value); }
        }

        string formDeviceOwnership = string.Empty;
        public string FormDeviceOwnership
        {
            get { return formDeviceOwnership; }
            set { SetProperty(ref formDeviceOwnership, value); }
        }

        string formArea = string.Empty;
        public string FormArea
        {
            get { return formArea; }
            set { SetProperty(ref formArea, value); }
        }

        string formGender = string.Empty;
        public string FormGender
        {
            get { return formGender; }
            set { SetProperty(ref formGender, value); }
        }

        string formIdNumber = string.Empty;
        public string FormIdNumber
        {
            get { return formIdNumber; }
            set { SetProperty(ref formIdNumber, value); }
        }

        string formMobileNumber = string.Empty;
        public string FormMobileNumber
        {
            get { return formMobileNumber; }
            set { SetProperty(ref formMobileNumber, value); }
        }

        string formAlternativeMobileNumber = string.Empty;
        public string FormAlternativeMobileNumber
        {
            get { return formAlternativeMobileNumber; }
            set { SetProperty(ref formAlternativeMobileNumber, value); }
        }

        string formAgentCode = string.Empty;
        public string FormAgentCode
        {
            get { return formAgentCode; }
            set { SetProperty(ref formAgentCode, value); }
        }

        string formUsername = string.Empty;
        public string FormUsername
        {
            get { return formUsername; }
            set { SetProperty(ref formUsername, value); }
        }

        string formPassword = string.Empty;
        public string FormPassword
        {
            get { return formPassword; }
            set { SetProperty(ref formPassword, value); }
        }

        int formProvinceId = 0;
        public int FormProvinceId
        {
            get { return formProvinceId; }
            set { SetProperty(ref formProvinceId, value); }
        }

        string formProvince = string.Empty;
        public string FormProvince
        {
            get { return formProvince; }
            set { SetProperty(ref formProvince, value); }
        }

        int formTownId = 0;
        public int FormTownId
        {
            get { return formTownId; }
            set { SetProperty(ref formTownId, value); }
        }

        string formTown = string.Empty;
        public string FormTown
        {
            get { return formTown; }
            set { SetProperty(ref formTown, value); }
        }

        int formSupervisorId = 0;
        public int FormSupervisorId
        {
            get { return formSupervisorId; }
            set { SetProperty(ref formSupervisorId, value); }
        }

        string formSupervisor = string.Empty;
        public string FormSupervisor
        {
            get { return formSupervisor; }
            set { SetProperty(ref formSupervisor, value); }
        }

        int formNationalityId = 0;
        public int FormNationalityId
        {
            get { return formNationalityId; }
            set { SetProperty(ref formNationalityId, value); }
        }

        string formNationality = string.Empty;
        public string FormNationality
        {
            get { return formNationality; }
            set { SetProperty(ref formNationality, value); }
        }

        #endregion

        private bool ValidatePassword(string password)
        {
            var input = password;
            var ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password shoud be at least 8 characters long, have a number, an upper and a lower case character";

                page.DisplayAlert("Password Error!", ErrorMessage, "OK");

                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password shoud be at least 8 characters long, have a number, an upper and a lower case character";

                page.DisplayAlert("Password Error!", ErrorMessage, "OK");

                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password shoud be at least 8 characters long, have a number, an upper and a lower case character";

                page.DisplayAlert("Password Error!", ErrorMessage, "OK");

                return false;
            }

            else if (!hasMinimum8Chars.IsMatch(input))
            {
                ErrorMessage = "Password shoud be at least 8 characters long, have a number, an upper and a lower case character";

                page.DisplayAlert("Password Error!", ErrorMessage, "OK");

                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            /* try
             {
                 strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                       RegexOptions.None, TimeSpan.FromMilliseconds(200));
             }
             catch (RegexMatchTimeoutException)
             {
                 return false;
             }
             */
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            throw new NotImplementedException();
        }
    }


    public class UserValidator : AbstractValidator<UserAccount>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.PhoneNumber).NotNull().Length(10, 12);
            RuleFor(x => x.Password).NotNull();
            RuleFor(x => x.ConfirmPassword).NotNull().Equal(x => x.Password);
            RuleFor(x => x.Email).NotNull().EmailAddress();
        }
    }

}

