using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using YomoneyApp.Models.AuthenticationDetail;
using YomoneyApp.Models.Nationality;
using YomoneyApp.Models.Province;
using YomoneyApp.Models.Supervisor;
using YomoneyApp.Models.Town;

namespace YomoneyApp.Services
{
    public class WebService
    {
        public async Task<TransactionResponse> GetResponse(string url, string content)
        {
            TransactionResponse transactionResponse = new TransactionResponse();

            try
            {
                var baseAddress = new Uri("https://www.yomoneyservice.com");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var httpClient = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    httpClient.Timeout = TimeSpan.FromMinutes(3);

                    cookieContainer.Add(baseAddress, new Cookie("AspxAutoDetectCookieSupport", "1", "/", "www.yomoneyservice.com"));
                    
                    string requestUri = string.Empty;

                    if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(content))
                    {
                        requestUri = string.Format(url, content);
                    }                    

                    var httpResponseMessage = await httpClient.GetAsync(requestUri);

                    var result = httpResponseMessage.Content.ReadAsStringAsync();

                    if (result != null)
                    {
                        transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(result.Result);                       
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return transactionResponse;
        }

        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

        public static async Task<List<Province>> GetProvincesAsync(string uri)
        {
            var provinces = new List<Province>();
            var authenticationDetail = new AuthenticationDetail(); 

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(uri);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, authenticationDetail?.Username, authenticationDetail?.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();                

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    provinces = JsonConvert.DeserializeObject<List<Province>>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return provinces;
        }

        public static async Task<List<Town>> GetTownsAsync(string uri)
        {
            var towns = new List<Town>();
            var authenticationDetail = new AuthenticationDetail();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(uri);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, authenticationDetail?.Username, authenticationDetail?.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    towns = JsonConvert.DeserializeObject<List<Town>>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return towns;
        }

        public static async Task<List<Nationality>> GetNationalitiesAsync(string uri)
        {
            var nationalities = new List<Nationality>();
            var authenticationDetail = new AuthenticationDetail();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(uri);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, authenticationDetail?.Username, authenticationDetail?.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    nationalities = JsonConvert.DeserializeObject<List<Nationality>>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return nationalities;
        }

        public static async Task<List<Supervisor>> GetSupervisorsAsync(string uri)
        {
            var supervisors = new List<Supervisor>();
            var authenticationDetail = new AuthenticationDetail();

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                    delegate { return true; }
                );

                string url = String.Format(uri);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Timeout = 120000;
                SetBasicAuthHeader(httpWebRequest, authenticationDetail?.Username, authenticationDetail?.Password);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    supervisors = JsonConvert.DeserializeObject<List<Supervisor>>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return supervisors;
        }

        public static void SetBasicAuthHeader(WebRequest request, string userName, string userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}
