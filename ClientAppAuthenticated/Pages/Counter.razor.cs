using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Count;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;

namespace ClientAppAuthenticated.Pages
{
    public partial class Counter
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public GrpcChannel Channel { get; set; }

        private int currentCount = 0;
        private CancellationTokenSource? cts;
        private string Token;
        private string Error;

        private async Task GetToken()
        {
            Token = await Http.GetStringAsync("/generateJwtToken?name=stef");
        }

        private async Task IncrementCount()
        {
            cts = new CancellationTokenSource();

            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {Token}");

            var client = new Count.Counter.CounterClient(Channel);
            var call = client.StartCounter(new CounterRequest() { Start = currentCount }, cancellationToken: cts.Token);

            try
            {
                Error = string.Empty;
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    currentCount = message.Count;
                    StateHasChanged();
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // Ignore exception from cancellation
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }

        private void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
    }
}
