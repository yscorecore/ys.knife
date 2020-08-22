using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace YS.Knife.Rest.Client
{
    public class RestClient
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RestInfo restInfo;
        private readonly HttpClient httpClient;
        public RestClient(string baseAddress, HttpClient httpClient)
            : this(new RestInfo
            {
                BaseAddress = baseAddress
            }, httpClient)
        {
        }
        public RestClient(RestInfo restInfo, HttpClient httpClient)
        {
            this.restInfo = restInfo;
            this.httpClient = httpClient;
            this.InitHttpClient();
        }

        public RestClient(IRestInfoFactory restInfoFactory, HttpClient httpClient)
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
            if (restInfo.DefaultHeaders != null)
            {
                foreach (var kv in restInfo.DefaultHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                }
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
            if (!string.IsNullOrEmpty(queryString))
            {
                if (requestPath.Contains("?"))
                {
                    requestPath = requestPath + queryString;
                }
                else
                {
                    requestPath = requestPath + "?" + queryString;
                }
            }

            var requestUri = new Uri(requestPath, UriKind.Relative);

            using (var request = new HttpRequestMessage(apiInfo.Method, requestUri))
            {
                this.AppendRequestHeader(apiInfo, request);
                this.AppendRequestJsonBody(apiInfo, request);
                //this.AppendRequestForm(apiInfo, request);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        //private void AppendRequestForm(ApiInfo apiInfo, HttpRequestMessage request)
        //{
        //    var formItem = apiInfo.Arguments.SingleOrDefault(p => p.Source == ArgumentSource.FormUrlEncoded);
        //    if (formItem != null)
        //    {
        //        var text = JsonSerializer.Serialize(formItem.Value);

        //        request.Content = new StringContent(text, Encoding.UTF8, "application/json");
        //    }
        //}

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
                var argument = apiInfo.Arguments.Single(p => p.Source == ArgumentSource.Router && nm.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
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
            var queryItems = apiInfo.Arguments.Where(p => p.Source == ArgumentSource.Query).ToList();
            if (queryItems.Count == 0) return string.Empty;
            var dic = new Dictionary<string, string>();
            foreach (var queryItem in queryItems)
            {
                // TODO array ,list 
                dic.Add(queryItem.Name, HttpUtility.UrlEncode(ValueToString(queryItem.Value)));
            }
            return string.Join("&", dic.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private void AppendRequestHeader(ApiInfo apiInfo, HttpRequestMessage httpRequestMessage)
        {
            var headerItems = apiInfo.Arguments.Where(p => p.Source == ArgumentSource.Header);
            foreach (var headerItem in headerItems)
            {
                httpRequestMessage.Headers.Add(headerItem.Name, ValueToString(headerItem.Value));
            }
        }
        private void AppendRequestJsonBody(ApiInfo apiInfo, HttpRequestMessage httpRequestMessage)
        {
            var bodyItem = apiInfo.Arguments.SingleOrDefault(p => p.Source == ArgumentSource.BodyJson);
            if (bodyItem != null)
            {
                var text = JsonSerializer.Serialize(bodyItem.Value);
                httpRequestMessage.Content = new StringContent(text, Encoding.UTF8, "application/json");
            }
        }
    }

}

