using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft;
using Newtonsoft.Json;

namespace Demo_WebAPI_Weather.DataAccessLayer
{
    public class RestApiClientSync : IRestApiClient
    {
        public WeatherData ExecuteRequest(RestClient restClient, IRestRequest request)
        {

            var response = restClient.Execute(request);
            var content = response.Content;
            WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(content);

            return weatherData;
        }
    }
}
