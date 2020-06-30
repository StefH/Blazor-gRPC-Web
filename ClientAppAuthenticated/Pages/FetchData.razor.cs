using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Weather;

namespace ClientAppAuthenticated.Pages
{
    [Authorize]
    public partial class FetchData
    {
        [Inject]
        GrpcChannel Channel { get; set; }

        [Inject]
        HttpClient Http { get; set; }

        private IList<WeatherForecast> forecasts;

        protected override async Task OnInitializedAsync()
        {
            var client = new WeatherForecasts.WeatherForecastsClient(Channel);
            forecasts = (await client.GetWeatherForecastsAsync(new Empty())).Forecasts;

            try
            {
                forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}