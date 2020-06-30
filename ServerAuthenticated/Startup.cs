#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Server.Services;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireClaim(ClaimTypes.Name);
                });
            });
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters =
            //            new TokenValidationParameters
            //            {
            //                ValidateAudience = false,
            //                ValidateIssuer = false,
            //                ValidateActor = false,
            //                ValidateLifetime = false
            //            };
            //    });

            // https://austincooper.dev/2020/02/02/azure-active-directory-authentication-in-asp.net-core-3.1/
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => config.Bind("AzureAd", options));

            //services.AddAuthorization();
            //services.AddAuthorizationPolicyEvaluator();
            //services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
            //    .AddAzureADBearer(options => config.Bind("AzureAd", options));

            services.AddControllers();

            services.AddGrpc();

            // https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-3.1#grpc-web-and-cors
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();

            app.UseRouting();

            // https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-3.1#grpc-web-and-cors
            app.UseCors();

            app.UseAuthentication(); // UseAuthentication must come before UseAuthorization
            app.UseAuthorization();

            app.UseGrpcWeb(); // Must be added between UseRouting and UseEndpoints
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UploadFileService>().EnableGrpcWeb();
                endpoints.MapGrpcService<WeatherService>().EnableGrpcWeb();


                endpoints.MapGrpcService<CounterService>().EnableGrpcWeb()
                    .RequireAuthorization() // https://blog.sanderaernouts.com/grpc-aspnetcore-azure-ad-authentication
                    .RequireCors("AllowAll"); // https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-3.1#grpc-web-and-cors

                endpoints.MapFallbackToFile("index.html");

                endpoints.MapControllers();
            });
        }
    }
}
