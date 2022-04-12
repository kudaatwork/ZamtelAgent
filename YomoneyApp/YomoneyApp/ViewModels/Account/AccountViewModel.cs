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

namespace YomoneyApp
{
    public class AccountViewModel : ViewModelBase
    {
        string HostDomain = "http://192.168.100.150:5000";
        //string ProcessingCode = "350000";
        IDataStore dataStore;

        public static int counter = 0;
        public static int answerCounter = 0;

        MapPageViewModel mapPageViewModel;
        ServiceViewModel serviceViewModel;
        CountryPickerViewModel countryPickerViewModel;
        CancellationTokenSource cts;

        public AccountViewModel(Page page) : base(page)
        {
            dataStore = DependencyService.Get<IDataStore>();
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
            Message = "Loading International No.s...";
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

                        SelectedCountry = CountryUtils.GetCountryModelByName(placemark.CountryName);
                   
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
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Password Error!", "Please enter your password", "OK");
                return;
            }

            Message = "Logging In...";
            IsBusy = true;
            loginCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = ActualPhoneNumber + ":" + password;
                trn.MTI = "0100";
                trn.ProcessingCode = "200000";

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";

                HttpClient client = new HttpClient();

                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "SignedIn")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        string resp = "";                        

                        try
                        {               
                            resp = ac.SaveCredentials(actualPhoneNumber, password).Result;

                            //PhoneNumber = null;
                            //ActualPhoneNumber = null;
                            Password = null;
                        }
                        catch
                        {
                            App.MyLogins = actualPhoneNumber;
                            App.AuthToken = password;
                            resp = "00000";
                        }
                        if (resp == "00000")
                        {
                            //await page.Navigation.PushAsync(new HomePage());

                            #region Commented Out Code

                            //if (!string.IsNullOrEmpty(response.Narrative))
                            //{
                            //    if (response.Narrative.ToUpper().Trim() == "TRUE") // Has Questions
                            //    {
                            //        if (!string.IsNullOrEmpty(response.Note))
                            //        {
                            //            char[] delimite = new char[] { ',' };

                            //            string[] parts = response.Note.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                            //            var accountStatus = parts[0].ToUpper().Trim();
                            //            var hasEmail = parts[1];

                            //            if (hasEmail.ToUpper().Trim() == "TRUE") // Has Email
                            //            {
                            //                switch (accountStatus)
                            //                {
                            //                    case "LOCKED":

                            //                        await page.DisplayAlert("Error", "You have exceeded the number of login attempts. Please contact customer support for help", "OK");

                            //                        await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");

                            //                        await page.Navigation.PushAsync(new SignIn());

                            //                        break;

                            //                    case "RESET":

                            //                        await page.Navigation.PushAsync(new AddEmailAddress(PhoneNumber));

                            //                        break;

                            //                    case "ACTIVE":

                            //                        await page.Navigation.PushAsync(new HomePage());

                            //                        break;

                            //                    default:
                            //                        await page.Navigation.PushAsync(new HomePage());
                            //                        break;
                            //                }
                            //            }
                            //            else
                            //            {
                            //                // Prompt user to enter Email
                            //                await page.Navigation.PushAsync(new AddEmailAddress(ActualPhoneNumber));
                            //            }
                            //        }
                            //        else
                            //        {
                            //            await page.DisplayAlert("Error!", "There has been an error from the server. Contact customer Support", "OK");

                            //            await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        // Prompt user to enter Questions
                            //        try
                            //        {
                            //            LoadQuestions();
                            //        }
                            //        catch (Exception e)
                            //        {
                            //            await page.DisplayAlert("Error", e.Message, "OK");
                            //        }
                            //    }
                            //}

                            #endregion

                            #region Zamtel Home Screen

                            #region Load Actions
                            //MenuItem menuItem = new MenuItem();

                            //menuItem.Id = "1";
                            //menuItem.Image = "http://192.168.100.150:5000/Content/Logos/ZAMTEL/zamtel.png";
                            //menuItem.Title = "SIM CARD MANAGEMENT";
                            //menuItem.Description = "SIM CARD MANAGEMENT";
                            //menuItem.Section = "Service";
                            //menuItem.Note = "ZAMTEL";
                            //menuItem.ServiceId = 1;
                            //menuItem.TransactionType = 12;
                            //menuItem.SupplierId = "5-0001-0001052";
                            ////menuItem.date = "0001-01-01T00:00:00";

                            //await page.Navigation.PushAsync(new ServiceActions(menuItem));
                            #endregion

                            #region Load Services

                            //if (!string.IsNullOrEmpty(response.Note))
                            //{
                            //    char[] delimite = new char[] { ',' };

                            //    string[] parts = response.Note.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                            //    var accountStatus = parts[0].ToUpper().Trim();

                            //    if (accountStatus.ToUpper() == "ACTIVE")
                            //    {

                            //    }
                            //}

                            MenuItem menuItem = new MenuItem();

                            menuItem.Id = "1";
                            menuItem.Image = "http://192.168.100.150:5000/Content/Logos/ZAMTEL/zamtel.png";
                            menuItem.Title = "ZAMTEL";
                            menuItem.Note = "BANKING";
                            menuItem.TransactionType = 12;
                            menuItem.SupplierId = "5-0001-0001052";
                            //menuItem.date = "0001-01-01T00:00:00";

                            await page.Navigation.PushAsync(new ProviderServices(menuItem));
                            #endregion

                            //await page.Navigation.PushAsync(new HomePage());

                            #endregion

                        }
                        else
                        {
                            //ResponseDescription = "Your device is not compatable";

                            await page.DisplayAlert("Login Error!", "Your device is not compatable", "OK");
                        }
                    }
                    else
                    {
                        ResponseDescription = "Invalid Username or Password";

                        await page.DisplayAlert("Login Error!", "Either your phone number or password is wrong", "OK");
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "An error occurred while sending the request to the server")
                {
                    await page.DisplayAlert("Login Error", ex.Message, "OK");
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

            if (string.IsNullOrEmpty(ContactName))
            {
                await page.DisplayAlert("Full Name Error!", "Please enter your Fullname and Surname", "OK");
                return;
            }

            string[] nam = ContactName.Split(' ');

            if (nam.Length < 2 || nam[0].Length < 3 || nam[1].Length < 3)
            {
                await page.DisplayAlert("Full Name Error!", "Please enter your Fullname and Surname no initials", "OK");
                return;
            }

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

            if (string.IsNullOrEmpty(Date.ToString()))
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

            if (string.IsNullOrEmpty(Password))
            {
                await page.DisplayAlert("Password Error!", "Please enter a password", "OK");
                return;
            }

            var isPasswordValid = ValidatePassword(password);

            if (!isPasswordValid)
            {
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                await page.DisplayAlert("Confirm Password Error!", "Please confirm password", "OK");
                return;
            }            

            if (confirmPassword != password)
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
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Name + "_" + ContactName + "_" + "NA" + "_" + ActualPhoneNumber + "_" + "NA" + "_" + Password + "_" + Date + "_" + Gender;

                Name = null;
                ContactName = null;                
                Date = DateTime.Now.Date;
                Password = null;
                ConfirmPassword = null;

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

                        string paramlocal = string.Format(HostDomain + "/Mobile/Create/?{0}", myContent);

                        string result = await client.GetStringAsync(paramlocal);

                        if (result != "System.IO.MemoryStream")
                        {
                            var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                            if (response.ResponseCode == "00000")
                            {
                                try
                                {
                                    AccessSettings ac = new Services.AccessSettings();

                                    App.MyLogins = actualPhoneNumber;
                                    App.AuthToken = password;

                                    MenuItem mn = new MenuItem();

                                    try
                                    {
                                        var resp = ac.SaveCredentials(actualPhoneNumber, password).Result;

                                        if (!string.IsNullOrEmpty(response.Note))
                                        {
                                            await page.DisplayAlert("Success!", "Your YoApp account has been created successfully!", "OK");

                                            MenuItem menuItem = new MenuItem();

                                            menuItem.Id = "1";
                                            menuItem.Image = "http://192.168.100.150:5000/Content/Logos/ZAMTEL/zamtel.png";
                                            menuItem.Title = "ZAMTEL";
                                            menuItem.Note = "BANKING";
                                            menuItem.TransactionType = 12;
                                            menuItem.SupplierId = "5-0001-0001052";
                                            //menuItem.date = "0001-01-01T00:00:00";

                                            await page.Navigation.PushAsync(new ProviderServices(menuItem));

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
                                            await page.DisplayAlert("Account Creation", "There is something wrong on creating your account. Contact Customer Support", "OK");

                                            await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        await page.DisplayAlert("Account Creation Error", "An error has occured whilst saving the transaction. Contact Customer Support", "OK");

                                        await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +260 787 800 013", "Email: support@zamtel.zm", "Call: +260 787 800 013");
                                    }
                                }
                                catch (Exception e)
                                {
                                    await page.DisplayAlert("Account Creation Error", "An error has occured. Check the application permissions and allow data storage or Contact Customer Support", "OK");

                                    await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +260 787 800 013", "Email: support@zamtel.zm", "Call: +260 787 800 013");
                                }
                            }
                            else
                            {
                                await page.DisplayAlert("Error", response.Description, "OK");

                                await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +260 787 800 013", "Email: support@zamtel.zm", "Call: +260 787 800 013");
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

        int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year - 1) +
                (((endDate.Month > startDate.Month) ||
                ((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);
        }

        #region AddEmail
        Command addEmailCommand;
        public Command AddEmailCommand
        {
            get
            {
                return addEmailCommand ??
                    (addEmailCommand = new Command(async () => await ExecuteAddEmailCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteAddEmailCommand()
        {
            if (IsBusy)
                return;

            //ActualPhoneNumber = SelectedCountry.CountryCode + PhoneNumber;

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
            addEmailCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Email + "_" + ActualPhoneNumber;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/UpdateEmail/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        if (!string.IsNullOrEmpty(response.Narrative))
                        {
                            if (response.Narrative.ToUpper().Trim() == "TRUE") // Has Questions
                            {
                                await page.Navigation.PushAsync(new HomePage());
                            }
                            else // Load Questions
                            {
                                try
                                {
                                    LoadQuestions();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);

                                    await page.DisplayAlert("Error!", "There has been an error in loading your questions. Contact Customer Support.", "OK");

                                    await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
                                }
                            }
                        }
                        else
                        {
                            await page.DisplayAlert("Info!", "Answer the questions that follow to improve your security on the App", "OK");

                            try
                            {
                                LoadQuestions();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);

                                await page.DisplayAlert("Error!", "There has been an error in loading your questions. Contact Customer Support.", "OK");

                                await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
                            }
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
                await page.DisplayAlert("Email Error", "There was an error in adding your emai address, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                addEmailCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region ProvideAnswers

        Command submitTheAnswerCommand;
        public Command SubmitTheAnswerCommand
        {
            get
            {
                return submitTheAnswerCommand ??
                    (submitTheAnswerCommand = new Command(async () => await ExecuteSubmitTheAnswerCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSubmitTheAnswerCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Answer))
            {
                await page.DisplayAlert("Answer Error", "Please enter your answer.", "OK");
                return;
            }

            Message = "Submiting Answer...";
            IsBusy = true;
            submitTheAnswerCommand?.ChangeCanExecute();

            try
            {
                if (answerCounter < 3)
                {
                    AnswerHandler();
                }
                else
                {
                    await page.DisplayAlert("Answer Error", "We have barred you any further attempts to answer your question. Try again later", "OK");
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Answer Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                submitTheAnswerCommand?.ChangeCanExecute();
            }
        }

        Command provideAnswerCommand;
        public Command ProvideAnswerCommand
        {
            get
            {
                return provideAnswerCommand ??
                    (provideAnswerCommand = new Command(async () => await ExecuteProvideAnswerCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteProvideAnswerCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Answer))
            {
                await page.DisplayAlert("Answer Error", "Please enter your answer.", "OK");
                return;
            }

            Message = "Submiting Answer...";
            IsBusy = true;
            provideAnswerCommand?.ChangeCanExecute();

            try
            {
                AccessSettings accessSettings = new AccessSettings();

                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = accessSettings.UserName + "_" + SecurityQuestion + "_" + Answer;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/SubmitAnswer/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        await page.DisplayAlert("Answer Successful", "Answer submitted successfully.", "OK");

                        if (counter < 3)
                        {
                            LoadQuestions();
                        }
                        else
                        {
                            if (response.Note != null)
                            {
                                if (response.Note.ToUpper().Trim() == "RESET")
                                {
                                    await page.Navigation.PushAsync(new PasswordReset(accessSettings.UserName, "NA"));
                                }
                                else
                                {
                                    await page.Navigation.PushAsync(new HomePage());
                                }
                            }
                            else
                            {
                                await page.Navigation.PushAsync(new HomePage());
                            }
                        }
                    }
                    else if (response.ResponseCode == "00090")
                    {
                        if (response.Note != null)
                        {
                            if (response.Note.ToUpper().Trim() == "RESET")
                            {
                                await page.Navigation.PushAsync(new PasswordReset(accessSettings.UserName, "NA"));
                            }
                            else
                            {
                                await page.Navigation.PushAsync(new HomePage());
                            }
                        }
                        else
                        {
                            await page.Navigation.PushAsync(new HomePage());
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
                await page.DisplayAlert("Answer Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                provideAnswerCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region Load Questions From Server
        public async void LoadQuestions()
        {
            TransactionRequest transactionRequest = new TransactionRequest();

            string transactionBody = "";

            transactionRequest.Narrative = PhoneNumber;

            transactionBody += "Narrative=" + transactionRequest.Narrative;

            HttpClient httpClient = new HttpClient();

            var bodyContent = transactionBody;

            string request = string.Format(HostDomain + "/Mobile/LoadQuestions/?{0}", bodyContent);

            string http = await httpClient.GetStringAsync(request);

            if (http != "System.IO.MemoryStream")
            {
                var httpResponse = JsonConvert.DeserializeObject<TransactionResponse>(http);

                if (httpResponse.ResponseCode == "00000")
                {
                    var deserilizedQuestions = JsonConvert.DeserializeObject<SecurityQuestions>(httpResponse.Narrative);

                    SecurityQuestion = deserilizedQuestions.Id + ". " + deserilizedQuestions.Question;

                    counter++;

                    await page.Navigation.PushAsync(new SecurityQuestion(phone, SecurityQuestion));
                }
            }
        }

        public async void LoadQuestion()
        {
            TransactionRequest transactionRequest = new TransactionRequest();

            string transactionBody = "";

            transactionRequest.Narrative = PhoneNumber;

            transactionBody += "Narrative=" + transactionRequest.Narrative;

            HttpClient httpClient = new HttpClient();

            var bodyContent = transactionBody;

            string request = string.Format(HostDomain + "/Mobile/LoadQuestion/?{0}", bodyContent);

            string http = await httpClient.GetStringAsync(request);

            if (http != "System.IO.MemoryStream")
            {
                var httpResponse = JsonConvert.DeserializeObject<TransactionResponse>(http);

                if (httpResponse.ResponseCode == "00000")
                {
                    var ques = new Models.Questions.Question();

                    ques.QuestionAndAnswer = httpResponse.Narrative;

                    char[] delimite = new char[] { '_' };

                    string[] parts = ques.QuestionAndAnswer.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                    SecurityQuestion = parts[0];

                    await page.Navigation.PushAsync(new Views.Login.Question(phone, SecurityQuestion));
                }
                else
                {
                    await page.DisplayAlert("Error", httpResponse.Description, "OK");

                    await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");

                }
            }
        }

        public async void AnswerHandler()
        {
            TransactionRequest trn = new TransactionRequest();
            trn.Narrative = PhoneNumber + "_" + SecurityQuestion + "_" + Answer;

            string Body = "";

            Body += "Narrative=" + trn.Narrative;

            HttpClient client = new HttpClient();

            var myContent = Body;

            string paramlocal = string.Format(HostDomain + "/Mobile/SubmitForgotAnswer/?{0}", myContent);

            string result = await client.GetStringAsync(paramlocal);

            if (result != "System.IO.MemoryStream")
            {
                var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                if (response.ResponseCode == "00000")
                {
                    await page.DisplayAlert("Answer Validation", "Answer validated successfully.", "OK");

                    await page.Navigation.PushAsync(new PasswordReset(PhoneNumber, "NA"));
                }
                else
                {
                    answerCounter++;
                    await page.DisplayAlert("Answer Error", "Please provide the answer you once provided", "OK");
                }
            }
        }
        #endregion

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
            addEmailCommand?.ChangeCanExecute();

            try
            {
                AccessSettings accessSettings = new AccessSettings();
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Email + "_" + accessSettings.UserName;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/EmailLookup/?{0}", myContent);

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
                        await page.DisplayActionSheet("Customer Support Contact Details", "Ok", "Cancel", "WhatsApp: +263 787 800 013", "Email: sales@yoapp.tech", "Skype: kaydizzym@outlook.com", "Call: +263 787 800 013");
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
                addEmailCommand?.ChangeCanExecute();
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
                password = "NA";
                trn.CustomerAccount = phone + ":" + password;
                trn.MTI = "0100";
                trn.Narrative = phone + "_" + password;
                string Body = "";
                Body += "Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&MTI=0100";
                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/ForgotPassword/?{0}", myContent);
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

                string paramlocal = string.Format(HostDomain + "/Mobile/PhoneNumberVerification/?{0}", myContent);

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

                string paramlocal = string.Format(HostDomain + "/Mobile/PhoneNumberVerification/?{0}", myContent);

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
            verifyEmailCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = ActualPhoneNumber;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/PhoneNumberVerification/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "00000")
                    {
                        try
                        {
                            await page.Navigation.PushAsync(new Views.Login.OTPPage(ActualPhoneNumber));
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
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

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

                phone = uname;

                trn.CustomerAccount = phone + ":" + password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        await page.DisplayAlert("Success", "OTP Successfully Verified!", "OK");

                       // HomeViewModel.fileUpload.FormId = HomeViewModel.fileUpload.FormId + 1;

                        if (!string.IsNullOrEmpty(HomeViewModel.fileUpload.RecordId))
                        {
                            //string weblink = "http://192.168.100.150:5000/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&ServiceId=" +
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
                                            //await page.DisplayAlert("File Upload", "File scanned and saved successfully", "OK");
                                            IsBusy = false;

                                            Device.BeginInvokeOnMainThread(async () =>
                                            {
                                                //viewModel.IsBusy = false;
                                                //FileImage.Source = null;                                               

                                                //page.Navigation.PopAsync();

                                                await page.Navigation.PushAsync(new WebviewHyubridConfirm("http://192.168.100.150:5000" + serverresult, "OTP Verification", false, null));
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
                            string weblink = "http://192.168.100.150:5000/Mobile/Forms?SupplierId=" + HomeViewModel.fileUpload.SupplierId + "&ServiceId=" +
                           HomeViewModel.fileUpload.ServiceId + "&ActionId=" + HomeViewModel.fileUpload.ActionId +
                           "&Customer=" + HomeViewModel.fileUpload.PhoneNumber +
                           "&FormNumber=" + HomeViewModel.fileUpload.FormId + "&CallType=FirstTime";

                            await page.Navigation.PushAsync(new WebviewHyubridConfirm(weblink, "OTP Verification", false, null, false));
                        }                       
                    }
                    else if (response.ResponseCode == "Error" || response.ResponseCode == "00008")
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

        Command verifyQuestionOTPCommand;
        public Command VerifyQuestionOTPCommand
        {
            get
            {
                return verifyQuestionOTPCommand ??
                    (verifyQuestionOTPCommand = new Command(async () => await ExecuteVerifyQuestionOTPCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteVerifyQuestionOTPCommand()
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
            verifyQuestionOTPCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        LoadQuestion();
                    }
                    else if (response.ResponseCode == "Error" || response.ResponseCode == "00008" || response.Note == "Fail")
                    {
                        mn.Note = phone;
                        await page.DisplayAlert("Error", "Could not verify your OTP", "OK");
                    }
                    else
                    {
                        mn.Note = phone;
                        await page.DisplayAlert("Error", "Could not verify your OTP", "OK");
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
                verifyQuestionOTPCommand?.ChangeCanExecute();
            }
        }

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
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            verifyPhoneOTPCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        // MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        await page.Navigation.PushAsync(new PasswordReset(PhoneNumber, ""));
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

                trn.CustomerAccount = phone + ":" + password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

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

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", Body);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if (response.ResponseCode == "Success" || response.ResponseCode == "00000")
                    {

                    }
                    else
                    {
                        //IsConfirm = false;
                        // Retry = true;
                        await page.DisplayAlert("OTP Error", "Either SMS gateway is down or request could not reach the server." + " Please try again ", "OK");
                    }

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
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter Verification Code", "Please enter the verification code sent to your mobile", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            VerifyCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + password;
                trn.CustomerMSISDN = phone;
                trn.Mpin = password;
                trn.CustomerAccount = PhoneNumber;
                trn.CustomerMSISDN = PhoneNumber;
                trn.MTI = "0100";
                trn.ProcessingCode = "220000";
                trn.Narrative = phone + "_" + password;

                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100";
                Body += "&Mpin=" + trn.Mpin;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {

                        // mn.Description = "Your new password will be sent to your email address which starts and ends as shown bellow";
                        //mn.Title = response.Narrative;
                        //mn.Note = phone;
                        MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

                        //await page.Navigation.PushAsync(new AddEmailAddress(mn));
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

            if (string.IsNullOrWhiteSpace(password))
            {
                await page.DisplayAlert("Enter Password", "Please enter a password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                await page.DisplayAlert("Confirm Password", "Please confirm password.", "OK");
                return;
            }

            if (confirmPassword != password)
            {
                await page.DisplayAlert("Confirm Password", "Confirmation password not matching password.", "OK");
                return;
            }

            var isPasswordValid = ValidatePassword(password);

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
                
                trn.Narrative = access.UserName + "_" + email + "_" + password;

                string Body = "";

                Body += "Narrative=" + trn.Narrative;

                HttpClient client = new HttpClient();

                var myContent = Body;

                string paramlocal = string.Format(HostDomain + "/Mobile/ResetPassword/?{0}", myContent);

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    MenuItem mn = new MenuItem();

                    if (response.ResponseCode == "00000")
                    {
                        AccessSettings ac = new Services.AccessSettings();

                        App.MyLogins = PhoneNumber;
                        App.AuthToken = Password;

                        var resp = ac.SaveCredentials(PhoneNumber, Password);

                        await page.DisplayAlert("Password Reset", "Password Reset Successful", "OK");

                        IsBusy = false;

                        await page.Navigation.PushAsync(new SignIn());
                    }
                    else
                    {
                        ResponseDescription = response.Description;

                        await page.DisplayAlert("Error!", ResponseDescription, "OK");

                        await page.Navigation.PushAsync(new SignIn());
                    }
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

                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

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

        string idNumber = string.Empty;
        public string IdNumber
        {
            get { return idNumber; }
            set { SetProperty(ref idNumber, value); }
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

        string actualPhoneNumber = string.Empty;
        public string ActualPhoneNumber
        {
            get { return actualPhoneNumber; }
            set { SetProperty(ref actualPhoneNumber, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
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

        string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        string gender = string.Empty;
        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value); }
        }

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

