using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YomoneyApp.Services
{
    public class AccessSettings
    {
        #region Credentials
        public async Task<string> SaveCredentials(string userName, string password)
        {
            string response = "";
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    App.MyLogins = userName;
                    App.AuthToken = password;
                    await SecureStorage.SetAsync("oauth_token", password);
                    await SecureStorage.SetAsync("userName", userName);
                    response = "00000";
                }
                catch (Exception ex)
                {
                    try
                    {
                        Application.Current.Properties["oauth_token"] = password;
                        Application.Current.Properties["userName"] = userName;
                        response = "00000";
                    }
                    catch
                    {
                        try
                        {
                            App.MyLogins = userName;
                            App.AuthToken = password;
                            response = "00000";
                        }
                        catch
                        {
                            response = "12";
                        }
                        
                    }
                   
                }

            }
            return response;
        }

        public async Task<string> SaveValue(string KeyName, string KeyValue)
        {
            string response = "";
            if (!string.IsNullOrWhiteSpace(KeyName) && !string.IsNullOrWhiteSpace(KeyValue))
            {

                try
                {
                    await SecureStorage.SetAsync(KeyName, KeyValue);

                    response = "00000";
                }
                catch (Exception ex)
                {
                    try
                    {
                        Application.Current.Properties[KeyName] = KeyValue;
                      
                    }
                    catch
                    {
                        response = "12";
                    }
                    
                }

            }
            return response;
        }

        public string UserName
        {
            get
            {
                string oauthToken = "";

                try
                {
                    oauthToken = GetSetting("userName").Result;

                    if(string.IsNullOrEmpty(oauthToken))
                    {
                        try
                        {
                            oauthToken = Application.Current.Properties["userName"].ToString();

                            if (string.IsNullOrEmpty(oauthToken))
                            {
                                oauthToken = App.MyLogins;
                            }
                        }
                        catch
                        {
                            oauthToken = App.MyLogins;
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        oauthToken = Application.Current.Properties["userName"].ToString();

                        if(string.IsNullOrEmpty(oauthToken))
                        {
                            oauthToken = App.MyLogins;
                        }
                    }
                    catch
                    {
                        oauthToken = App.MyLogins;
                    }
                }

                return oauthToken;
            }
        }

        public string Password
        {
            get
            {
                string oauthToken = "";

                try
                {
                    oauthToken = GetSetting("oauth_token").Result;

                    if (string.IsNullOrEmpty(oauthToken))
                    {
                        try
                        {
                            oauthToken = Application.Current.Properties["oauth_token"].ToString();

                            if (string.IsNullOrEmpty(oauthToken))
                            {
                                oauthToken = App.AuthToken;
                            }
                        }
                        catch
                        {
                            oauthToken = App.AuthToken;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Possible that device doesn't support secure storage on device.
                    try
                    {
                        oauthToken = Application.Current.Properties["oauth_token"].ToString();

                        if (string.IsNullOrEmpty(oauthToken))
                        {
                            oauthToken = App.AuthToken;
                        }
                    }
                    catch
                    {
                        oauthToken = App.AuthToken;
                    }
                }
                return oauthToken;
            }
        }
        
        public  void DeleteCredentials()
        {            
            SecureStorage.Remove("UserName");
            SecureStorage.Remove("oauth_token");
        }

        public async Task<string> GetSetting(string key)
        {
            try
            {
                var st = await SecureStorage.GetAsync(key);

                if (st == null)
                {
                    st = Application.Current.Properties[key].ToString();

                    if(st == null)
                    {
                        st = App.AuthToken;
                    }
                }

                return st;
            }
            catch
            {
                try
                {
                   return  Application.Current.Properties[key].ToString();
                }
                catch
                {
                    return  App.AuthToken; 
                }
            }
        }

        #endregion
    }
}
