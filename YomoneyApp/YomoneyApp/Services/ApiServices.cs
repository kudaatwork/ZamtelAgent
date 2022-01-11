using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YomoneyApp.Constants;
using YomoneyApp.Models;

namespace YomoneyApp.Services
{
    public class ApiServices
    {
        private JsonSerializer _serializer = new JsonSerializer();

        private static ApiServices _ServiceClientInstance;

        public static ApiServices ServiceClientInstance
        {
            get
            {
                if (_ServiceClientInstance == null)
                    _ServiceClientInstance = new ApiServices();
                return _ServiceClientInstance;
            }
        }
        private HttpClient client;
        public ApiServices()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
        }

        public async Task<GoogleDirection> GetDirections(string originAddress, string destinationAddress, string mode, string waypoints)
        {
            try
            {
                GoogleDirection googleDirection = new GoogleDirection();

                var request = string.Empty;

                if (string.IsNullOrEmpty(waypoints))
                {
                    request = $"api/directions/json?mode={mode}&transit_routing_preference=less_driving&origin={originAddress}&destination={destinationAddress}&key={AppConstants.GoogleMapsApiKey}";
                }
                else
                {
                    request = $"api/directions/json?mode={mode}&transit_routing_preference=less_driving&origin={originAddress}&destination={destinationAddress}&waypoints=optimize:true{waypoints}&key={AppConstants.GoogleMapsApiKey}";
                }                

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        googleDirection = await Task.Run(() =>
                           JsonConvert.DeserializeObject<GoogleDirection>(json)
                        ).ConfigureAwait(false);

                    }
                }

                return googleDirection;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }            
        }
    }
}
