using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace YS.Knife.Extensions.Configuration.Api
{
    internal class ApiConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly Timer _timer;
        private readonly ApiConfigurationSource _apiConfigurationSource;
        protected string RemoteConfigurationData { get; set; }

        public ApiConfigurationProvider(ApiConfigurationSource apiConfigurationSource)
        {
            _apiConfigurationSource = apiConfigurationSource;
            _timer = new Timer(x => Load(),
                null,
                _apiConfigurationSource.Period,
                _apiConfigurationSource.Period);
        }

        public void Dispose()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            Console.WriteLine("Dispose timer");
        }

        public override void Load()
        {
            if (GetRemoteConfigurationString(out var remoteJsonText))
            {
                if (RemoteConfigurationData != remoteJsonText)
                {
                    RemoteConfigurationData = remoteJsonText;
                    Data = JsonSerializer.Deserialize<Dictionary<string, string>>(remoteJsonText);
                    OnReload();
                }
            }
        }



        private bool GetRemoteConfigurationString(out string text)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var resp = client.GetAsync(_apiConfigurationSource.ApiUrl).ConfigureAwait(false).GetAwaiter().GetResult();
                    text = resp.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (Exception)
            {
                if (!_apiConfigurationSource.Optional)
                {

                    throw;
                }
                text = default;
                return false;
            }

        }



    }
}
