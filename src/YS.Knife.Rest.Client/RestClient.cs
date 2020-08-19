using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YS.Knife.Rest.Client
{
    public class ClientBase
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RestInfo restInfo;
        private readonly HttpClient httpClient;
        public ClientBase(string baseAddress, HttpClient httpClient)
            : this(new RestInfo
            {
                BaseAddress = baseAddress
            }, httpClient)
        {
        }
        public ClientBase(RestInfo restInfo, HttpClient httpClient)
        {
            this.restInfo = restInfo;
            this.httpClient = httpClient;
            this.InitHttpClient();
        }

        public ClientBase(IRestInfoFactory restInfoFactory, HttpClient httpClient)
        {
            this.restInfo = restInfoFactory.GetRestInfo(this.GetType());
            this.httpClient = httpClient;
            this.InitHttpClient();
        }

        private void InitHttpClient()
        {
            if (restInfo == null) return;
            if (restInfo.MaxResponseContentBufferSize > 0)
            {
                httpClient.MaxResponseContentBufferSize = restInfo.MaxResponseContentBufferSize;
            }
            if (restInfo.Timeout.TotalSeconds > 0)
            {
                httpClient.Timeout = restInfo.Timeout;
            }
            if (!string.IsNullOrEmpty(restInfo.BaseAddress))
            {
                httpClient.BaseAddress = new Uri(restInfo.BaseAddress);
            }
        }


        public Task SendHttp(ApiInfo apiInfo)
        {
            return SendHttpAsResponse(apiInfo);
        }
        protected virtual async Task<HttpResponseMessage> SendHttpAsResponse(ApiInfo apiInfo)
        {
            _ = apiInfo ?? throw new ArgumentNullException(nameof(apiInfo));


            var requestPath = TranslatePath(apiInfo, apiInfo.Path);
            var queryString = BuildQueryString(apiInfo);
            var requestUri = new Uri(requestPath + queryString, UriKind.Relative);

            using (var request = new HttpRequestMessage(apiInfo.Method, requestUri))
            {
                this.AppendRequestHeader(apiInfo, request);
                this.AppendRequestBody(apiInfo, request);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
            }

        }

        public async Task<T> SendHttp<T>(ApiInfo apiInfo)
        {
            var response = await this.SendHttpAsResponse(apiInfo);
            return FromResponse<T>(response);
        }
        private T FromResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = response.Content;
            var responseData = content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(responseData))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(responseData, JsonOptions);
        }

        private string TranslatePath(ApiInfo apiInfo, string path)
        {

            // replace {key},{key:int},{age:range(18,120)},{ssn:regex(^\d{{3}}-\d{{2}}-\d{{4}}$)}
            var path2 = Regex.Replace(path, "\\{(?<nm>\\w+)(\\?)?(:.+)?\\}", (m) =>
            {
                var nm = m.Groups["nm"].Value;
                var argument = apiInfo.Arguments.Single(p => p.Source == ArgumentSource.FromRouter && nm.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
                return ValueToString(argument.Value);
            });
            return path2;
        }
        private static string ValueToString(object value)
        {
            if (value == null) return string.Empty;
            if (value is string) return value as string;
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(value);
            return converter.ConvertToInvariantString(value);
        }


        private string BuildQueryString(ApiInfo apiInfo)
        {
            var queryItems = apiInfo.Arguments.Where(p => p.Source == ArgumentSource.FromQuery).ToList();
            if (queryItems.Count == 0) return string.Empty;
            var queryString = QueryString.Empty;
            foreach (var queryItem in queryItems)
            {
                // TODO array ,list 
                queryString = queryString.Add(queryItem.Name, ValueToString(queryItem.Value));
            }
            return queryString.ToUriComponent();
        }

        private void AppendRequestHeader(ApiInfo apiInfo, HttpRequestMessage httpRequestMessage)
        {
            var headerItems = apiInfo.Arguments.Where(p => p.Source == ArgumentSource.FromHeader);
            foreach (var headerItem in headerItems)
            {
                httpRequestMessage.Headers.Add(headerItem.Name, ValueToString(headerItem.Value));
            }
        }
        private void AppendRequestBody(ApiInfo apiInfo, HttpRequestMessage httpRequestMessage)
        {
            var bodyItem = apiInfo.Arguments.SingleOrDefault(p => p.Source == ArgumentSource.FromBody);
            if (bodyItem != null)
            {
                var text = JsonSerializer.Serialize(bodyItem.Value);

                httpRequestMessage.Content = new StringContent(text, Encoding.UTF8, "application/json");
            }
        }
    }

}

