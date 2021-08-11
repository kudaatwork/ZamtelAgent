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
using System.Text.RegularExpressions;
using FluentValidation;
using System.Net;

namespace YomoneyApp
{
    public class AccountViewModel : ViewModelBase
    {
        string HostDomain = "https://www.yomoneyservice.com";
        //string ProcessingCode = "350000";
        IDataStore dataStore;
        public AccountViewModel(Page page) : base(page)
        {
            dataStore = DependencyService.Get<IDataStore>();
            Title = "Account";
        }

    
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

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await page.DisplayAlert("Enter Mobile Number", "Please enter a valid mobile number. e.g 263775555000", "OK");
                return;
            }
            else if(PhoneNumber.Length != 12 )
            {
                await page.DisplayAlert("Enter Mobile Number", "Please enter a valid mobile number. e.g 263775555000", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                await page.DisplayAlert("Enter Password", "Please enter a password.", "OK");
                return;
            }

            Message = "Logging In...";
            IsBusy = true;
            loginCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + password;
                trn.MTI = "0100";
                trn.ProcessingCode = "200000";

                string Body = "";
                Body += "CustomerAccount=" + trn.CustomerAccount;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&MTI=0100" ;

                HttpClient client = new HttpClient();

                var myContent = Body;
                string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

                    if(response.ResponseCode == "SignedIn")
                    {
                        AccessSettings ac = new Services.AccessSettings();
                        string resp = "";
                        try
                        {
                            resp = ac.SaveCredentials(phone, password).Result;
                           
                        }
                        catch
                        {
                            App.MyLogins = phone;
                            App.AuthToken = password;
                            resp = "00000";
                        }
                        if (resp == "00000")
                        {
                            await page.Navigation.PushAsync(new HomePage());
                        }
                        else
                        {
                            ResponseDescription = "Your device is not compatable";
                        }
                    }
                    else
                    {
                        ResponseDescription = "Invalid Username or Password";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "An error occurred while sending the request")
                {
                    await page.DisplayAlert("Sign Error", "An error has occured. Check your internet connection and retry", "OK");
                }
                else
                {
                    await page.DisplayAlert("Sign Error", "An error has occured. Check the application permissions and allow data storage", "OK");
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
                    (joinCommand = new Command(async () => await ExecutejoinCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecutejoinCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Name))
            {
                await page.DisplayAlert("Enter Username", "Please enter your username.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(ContactName))
            {
                await page.DisplayAlert("Enter Full Name", "Please enter your Fullname and Surname.", "OK");
                return;
            }

            string[] nam = ContactName.Split(' ');

            if (nam.Length < 2 || nam[0].Length <3 || nam[1].Length < 3)
            {
                await page.DisplayAlert("Enter Full Name", "Please enter your Fullname and Surname no initials.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                await page.DisplayAlert("Enter Password", "Please enter a password.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(confirmPassword ))
            {
                await page.DisplayAlert("Confirm Password", "Please confirm password.", "OK");
                return;
            }
            if (confirmPassword != password)
            {
                await page.DisplayAlert("Confirm Password", "Confirmation password not matching password.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber) )
            {
                await page.DisplayAlert("Enter Phone Number", "Please enter a valid mobile number.", "OK");
                return;
            }
            else if (PhoneNumber.Length != 12 && PhoneNumber.Length != 10 && PhoneNumber.Length != 9)
            {
                await page.DisplayAlert("Enter Phone Number", "Please enter a valid mobile number e.g 263777718713.", "OK");
                return;
            }

            Message = "Creating Account...";
            IsBusy = true;
            joinCommand?.ChangeCanExecute();

            try
            {
                TransactionRequest trn = new TransactionRequest();
                trn.Narrative = Name + "_" + ContactName + "_" + "NA" + "_" + PhoneNumber + "_" + "NA" + "_" + Password;

                await page.Navigation.PushAsync(new VerificationPage(PhoneNumber));
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
                                    App.MyLogins = phone;
                                    App.AuthToken = password;

                                    MenuItem mn = new MenuItem();

                                    try
                                    {
                                        var resp = ac.SaveCredentials(phone, password).Result;

                                        await page.Navigation.PushAsync(new AddEmailAddress(mn));
                                    }
                                    catch (Exception e)
                                    {
                                        await page.DisplayAlert("Join Error", "An error has occured whilst saving the transaction", "OK");
                                    }
                                    //await page.Navigation.PushAsync(new AddEmailAddress(MenuItem mn));
                                }
                                catch (Exception e)
                                {
                                    await page.DisplayAlert("Join Error", "An error has occured. Check the application permissions and allow data storage", "OK");
                                }
                                // await page.DisplayAlert("Account Creation", "Unable to create account, please try again.", "OK");
                            }
                            else
                            {
                                await page.DisplayAlert("Error", response.Description, "OK");
                                //ResponseDescription = "Invalid Username or Password";
                            }

                        }
                    }else
                    {
                        message = "Please enter a valid verification code";
                        await page.DisplayAlert("Join Error", message, "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Join Error", "Unable to create account, please check your internet connection and try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                joinCommand?.ChangeCanExecute();
            }

           // await page.Navigation.PopAsync();

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

        #region Verify

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
                        await page.DisplayAlert("Transaction Error", response.Description + " please try again ", "OK");
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

                       // await page.Navigation.PushAsync(new AddEmailAddress(mn));
                    }

                    else if(response.ResponseCode == "Error" || response.ResponseCode == "00008")
                    {
                        //mn.Description = "You need a valid email address for password reset please contact customer service";
                        mn.Note = phone;
                        await page.DisplayAlert("OTP Verification", response.Description, "OK");
                    }

                    else
                    {                        
                        //mn.Description = "Please enter an email address for your account where you new password will be sent";
                        mn.Note = phone;
                        await page.DisplayAlert("OTP Verification", mn.Description, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Oh Oooh :(", "Unable to verify code . Please check your internet connection and  try later", "OK");
            }
            finally
            {
                IsBusy = false;
                loginCommand?.ChangeCanExecute();
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
            if (string.IsNullOrWhiteSpace(Email))
            {
                await page.DisplayAlert("Enter Email", "Please enter a valid email address", "OK");
                return;
            }

            Message = "Processing...";
            IsBusy = true;
            ResetCommand?.ChangeCanExecute();

            try
            {
                List<MenuItem> mnu = new List<MenuItem>();
                TransactionRequest trn = new TransactionRequest();

                trn.CustomerAccount = phone + ":" + password;
                trn.MTI = "0100";
                trn.Narrative = phone  +"_" + email;
                string Body = "";
                Body += "Narrative=" + trn.Narrative;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&MTI=0100";
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
                        
                        await page.Navigation.PushAsync(new SignIn());
                    }
                    else
                    {
                        ResponseDescription = response.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await page.DisplayAlert("Oh Oooh :(", "Unable to reset password. Check you internet connection and  try again", "OK");
            }
            finally
            {
                IsBusy = false;
                loginCommand?.ChangeCanExecute();
            }
        }
        #endregion

        #region model
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

        string email = string.Empty;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        string password = string.Empty;
        public string Password
        {
            get { return password ; }
            set { SetProperty(ref password, value); }
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

