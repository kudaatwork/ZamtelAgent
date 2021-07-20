using Newtonsoft.Json;
using RetailKing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YomoneyApp.Services
{
    public class PostTransaction
    {
        public async Task<TransactionResponse> YomoneyDataStore(TransactionRequest trn)
        {
            TransactionResponse response = new TransactionResponse();
            try
            {
                string Body = "";
                Body += "CustomerMSISDN=" + trn.CustomerMSISDN;
                Body += "&CustomerAccount=" + trn.CustomerAccount;
                Body += "&AgentCode=" + trn.AgentCode;
                Body += "&Action=Mobile";
                Body += "&TerminalId=" + trn.TerminalId;
                Body += "&TransactionRef=" + trn.TransactionRef;
                Body += "&ServiceId=" + trn.ServiceId;
                Body += "&Product=" + trn.Product;
                Body += "&Amount=" + trn.Amount;
                Body += "&MTI=" + trn.MTI;
                Body += "&ProcessingCode=" + trn.ProcessingCode;
                Body += "&ServiceProvider=" + trn.ServiceProvider;
                Body += "&Narrative=" + trn.Narrative;
                Body += "&CustomerData=" + trn.CustomerData;
                Body += "&Note=" + trn.Note;
                Body += "&Mpin=" + trn.Mpin;


                HttpClient client = new HttpClient();
                var myContent = Body;
                string paramlocal = string.Format("http://192.168.100.150:5000/Mobile/Transaction/?{0}", myContent);
                string result = await client.GetStringAsync(paramlocal);
                /*HttpResponseMessage wcfResponse = await client.GetAsync(paramlocal);
                HttpContent stream = wcfResponse.Content;
                var data = stream.ReadAsStreamAsync();
                string result = data.Result.ToString();*/
                if (result != "System.IO.MemoryStream")
                {
                    response = JsonConvert.DeserializeObject<TransactionResponse>(result);
                }
                return response;
                
            }
            catch (Exception ex)
            {
                response.ResponseCode = "Error";
                response.Description = ex.Message;
                return response;
            }
            
          //  return response;
        }
    }
}
