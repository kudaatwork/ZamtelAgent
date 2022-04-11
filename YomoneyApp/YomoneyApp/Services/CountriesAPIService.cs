using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using YomoneyApp.Models;

namespace YomoneyApp.Services
{
    public class CountriesAPIService
    {
        public async Task<List<CountriesModel>> GetCountries()
        {
            try
            {
                HttpClient client = new HttpClient();

                var myContent = "";
                string paramlocal = string.Format("https://restcountries.com/v2/all?fields=name,callingCodes,flags,alpha2Code", myContent);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                string result = await client.GetStringAsync(paramlocal);

                if (result != "System.IO.MemoryStream")
                {
                    var countries = JsonConvert.DeserializeObject<List<CountriesModel>>(result);
                    return countries;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return null;
            }
        }

        public List<CountriesModel> GetCountriesList()
        {
            List<CountriesModel> countries = new List<CountriesModel>();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );                

                string url = String.Format("https://restcountries.com/v2/all?fields=name,callingCodes,flags,alpha2Code");
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = 120000;
                //SetBasicAuthHeader(httpWebRequest, eSolutionsCredentials.Username, eSolutionsCredentials.Password);
                //httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (Stream stream = httpResponse.GetResponseStream())

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            countries = JsonConvert.DeserializeObject<List<CountriesModel>>(result);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, e);
                //return null;
            }

            return countries;
        }
    }
}
