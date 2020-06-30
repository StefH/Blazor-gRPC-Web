using System;
using System.Text.Json;
using System.Threading.Tasks;
using ClientAppAuthenticated;
using Microsoft.JSInterop;

namespace Blazored.SessionStorage
{
    public class MySessionStorage : IMySessionStorage
    {
        private readonly IJSRuntime _jSRuntime;

        public MySessionStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task<string> GetStringAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        }

        public async Task<string> GetLocalStringAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
    }
}
