﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Count;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorWasmGrpcWithAADAuth.Client.Pages
{
    public class Model
    {
        public string Token { get; set; }
    }

    public partial class Counter
    {
        [Inject]
        public NavigationManager _navigation { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public GrpcChannel Channel { get; set; }

        private int currentCount = 0;
        private CancellationTokenSource? cts;
        public Model Model = new Model();
        private string Error;

        [Inject]
        public IAccessTokenProvider TokenProvider { get; set; }

        private async Task IncrementCount()
        {
            cts = new CancellationTokenSource();

            var tokenResult = await TokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = new[] { "821eb724-edb8-4dba-b425-3f953250c0ae/API.Access" }
            });

            if (tokenResult.TryGetToken(out var token))
            {
                Model.Token = token.Value;
            }
            else
            {
                Model.Token = "!!";
                _navigation.NavigateTo(tokenResult.RedirectUrl);
            }

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {Model.Token}" }
            };

            var client = new Count.Counter.CounterClient(Channel);
            var call = client.StartCounter(new CounterRequest() { Start = currentCount }, headers, cancellationToken: cts.Token);

            try
            {
                Error = string.Empty;
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    currentCount = message.Count;
                    StateHasChanged();
                }
            }
            catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.Cancelled)
            {
                // Ignore exception from cancellation
                Error = rpcException.Message;
            }
            catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.Unauthenticated)
            {
                Error = rpcException.Message;
                
            }
            catch (Exception exception)
            {
                Error = exception.Message;
            }
        }

        private void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
    }
}
