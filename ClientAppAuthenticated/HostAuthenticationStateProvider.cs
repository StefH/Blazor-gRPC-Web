﻿//using BlazorWasmCookieAuth.Shared.Authorization;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace BlazorWasmCookieAuth.Client.Services
//{
//    public class HostAuthenticationStateProvider : AuthenticationStateProvider
//    {
//        private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

//        private const string LogInPath = "Account/Login";
//        private const string LogOutPath = "Account/Logout";

//        private readonly NavigationManager _navigation;
//        private readonly HttpClient _client;
//        private readonly ILogger<HostAuthenticationStateProvider> _logger;

//        private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
//        private ClaimsPrincipal _cachedUser = new ClaimsPrincipal(new ClaimsIdentity());

//        public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<HostAuthenticationStateProvider> logger)
//        {
//            _navigation = navigation;
//            _client = client;
//            _logger = logger;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync() => new AuthenticationState(await GetUser(useCache: true));

//        public void SignIn(string customReturnUrl = null)
//        {
//            var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
//            var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
//            var logInUrl = _navigation.ToAbsoluteUri($"{LogInPath}?returnUrl={encodedReturnUrl}");
//            _navigation.NavigateTo(logInUrl.ToString(), true);
//        }

//        public void SignOut()
//        {
//            _navigation.NavigateTo(_navigation.ToAbsoluteUri(LogOutPath).ToString(), true);
//        }

//        private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
//        {
//            var now = DateTimeOffset.Now;
//            if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
//            {
//                _logger.LogDebug("Taking user from cache");
//                return _cachedUser;
//            }

//            _logger.LogDebug("Fetching user");
//            _cachedUser = await FetchUser();
//            _userLastCheck = now;

//            return _cachedUser;
//        }

//    }
//}