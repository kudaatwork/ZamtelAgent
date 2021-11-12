using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YomoneyApp.ViewModels.CustomerStatus
{
    public class CustomerStatusViewModel : ViewModelBase
    {
        string HostDomain = "https://www.yomoneyservice.com";

        public CustomerStatusViewModel(Page page) : base(page)
        {
            
        }

        #region Check Customer Status      
        //Command otpLogin;
        //public Command OtpLogin
        //{
        //    get
        //    {
        //        return otpLogin ??
        //            (otpLogin = new Command(async () => await ExecuteOtpLoginCommand(), () => { return !IsBusy; }));
        //    }
        //}

        //async Task ExecuteOtpLoginCommand()
        //{
        //    if (IsBusy)
        //        return;

        //    if (string.IsNullOrWhiteSpace(Password))
        //    {
        //        await page.DisplayAlert("Enter OTP", "Please enter your One-Time Password.", "OK");
        //        return;
        //    }

        //    Message = "Verifying One-Time Password...";
        //    IsBusy = true;
        //    otpLogin?.ChangeCanExecute();

        //    try
        //    {
        //        List<MenuItem> mnu = new List<MenuItem>();
        //        TransactionRequest trn = new TransactionRequest();

        //        trn.CustomerAccount = PhoneNumber + ":" + Password;
        //        trn.Mpin = Password;
        //        trn.CustomerAccount = PhoneNumber;
        //        trn.CustomerMSISDN = PhoneNumber;
        //        trn.MTI = "0100";
        //        trn.ProcessingCode = "220000";
        //        trn.Narrative = PhoneNumber + "_" + Password;

        //        string Body = "";

        //        Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
        //        Body += "&Narrative=" + trn.Narrative;
        //        Body += "&CustomerAccount=" + trn.CustomerAccount;
        //        Body += "&ProcessingCode=" + trn.ProcessingCode;
        //        Body += "&MTI=0100";
        //        Body += "&Mpin=" + trn.Mpin;

        //        HttpClient client = new HttpClient();

        //        var myContent = Body;

        //        string paramlocal = string.Format(HostDomain + "/Mobile/Transaction/?{0}", myContent);

        //        string result = await client.GetStringAsync(paramlocal);

        //        if (result != "System.IO.MemoryStream")
        //        {
        //            var response = JsonConvert.DeserializeObject<TransactionResponse>(result);

        //            MenuItem mn = new MenuItem();

        //            if (response.ResponseCode == "00000")
        //            {
        //                MessagingCenter.Send<string, string>("VerificationRequest", "VerifyMsg", "Verified");

        //                //await page.Navigation.PushAsync(new AddEmailAddress(mn));
        //            }
        //            else if (response.ResponseCode == "Error" || response.ResponseCode == "00008")
        //            {
        //                //mn.Description = "You need a valid email address for password reset please contact customer service";
        //                mn.Note = phone;
        //                await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
        //            }
        //            else
        //            {
        //                //mn.Description = "Please enter an email address for your account where you new password will be sent";
        //                mn.Note = phone;
        //                await page.DisplayAlert("OTP Verification", "There has been an error in verifying your One-Time-Password", "OK");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        await page.DisplayAlert("Error!", "There has been an error in submitting your OTP. Please try again", "OK");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //        otpLogin?.ChangeCanExecute();
        //    }
        //}

        #endregion

        #region Model

        string customerStatus = string.Empty;
        public string CustomerStatus
        {
            get { return customerStatus; }
            set { SetProperty(ref customerStatus, value); }
        }
        #endregion
    }
}
