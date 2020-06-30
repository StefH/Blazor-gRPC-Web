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
        private readonly IJSInProcessRuntime _jSInProcessRuntime;
        private readonly JsonSerializerOptions _jsonOptions;

        public event EventHandler<ChangingEventArgs> Changing;
        public event EventHandler<ChangedEventArgs> Changed;

        public MySessionStorage(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            
            _jSInProcessRuntime = jSRuntime as IJSInProcessRuntime;
        }


        public async Task<string> GetStringAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _jSRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
        }
    }
}
