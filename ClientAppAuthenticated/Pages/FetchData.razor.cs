using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Weather;

namespace ClientAppAuthenticated.Pages
{
    [Authorize]
    public partial class FetchData
    {
        [Inject]
        GrpcChannel Channel { get; set; }
        //public HttpClient Http { get; set; }

        private IList<WeatherForecast>? forecasts;

        protected override async Task OnInitializedAsync()
        {
            var client = new WeatherForecasts.WeatherForecastsClient(Channel);
            forecasts = (await client.GetWeatherForecastsAsync(new Empty())).Forecasts;

            //var client2 = new FileUpload.FileUploadClient(Channel);
            //var response = await client2.UploadAsync(new UploadRequest { });
        }

        //private WeatherForecast[] forecasts;

        //protected override async Task OnInitializedAsync()
        //{
        //    forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        //}

        //public class WeatherForecast
        //{
        //    public DateTime Date { get; set; }

        //    public int TemperatureC { get; set; }

        //    public string Summary { get; set; }

        //    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        //}
    }
}
