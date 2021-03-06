using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmGrpcWithAADAuth.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("BlazorWasmGrpcWithAADAuth.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWasmGrpcWithAADAuth.ServerAPI"));

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("821eb724-edb8-4dba-b425-3f953250c0ae/API.Access");
            });

            builder.Services.AddSingleton(services =>
            {
                // Get the service address from appsettings.json
                //var config = services.GetRequiredService<IConfiguration>();
                //var backendUrl = config["BackendUrl"];

                // Create a channel with a GrpcWebHandler that is addressed to the backend server.
                // GrpcWebText is used because server streaming requires it.
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());//new HttpClientHandler()
                return GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = httpHandler });
            });

            await builder.Build().RunAsync();
        }
    }
}
