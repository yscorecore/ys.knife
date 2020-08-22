using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
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
        #region 构造函数
        public RestClient(string baseAddress, HttpClient httpClient)
            : this(new RestInfo
            {
                BaseAddress = baseAddress
            }, httpClient)
        {
        }
        public RestClient(RestInfo restInfo, HttpClient httpClient)
        {
            _ = restInfo ?? throw new ArgumentNullException(nameof(restInfo));
            this.restInfo = restInfo;
            this.httpClient = httpClient;
        }
        public RestClient(IRestInfoFactory restInfoFactory, HttpClient httpClient)
        {
            this.restInfo = restInfoFactory.GetRestInfo(this.GetType());
            this.httpClient = httpClient;
        }
        #endregion


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


            using (var request = CreateRequestMessage(apiInfo.Method, requestPath))
            {
                this.AppendRequestHeader(apiInfo, request);
                this.AppendRequestJsonBody(apiInfo, request);
                this.AppendRequestUrlEncodeForm(apiInfo, request);
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        private void AppendRequestUrlEncodeForm(ApiInfo apiInfo, HttpRequestMessage request)
        {
            var bodyItem = apiInfo.Arguments.SingleOrDefault(p => p.Source == ArgumentSource.FormUrlEncoded);
            if (bodyItem != null)
            {
                var kvs = ObjectToStringKeyValuePairs(bodyItem.Value);
                request.Content = new FormUrlEncodedContent(kvs);
            }
        }



        private Uri CombinUri(string baseAddress, string requestPath)
        {
            if (string.IsNullOrEmpty(baseAddress))
            {
                return new Uri(requestPath, UriKind.Relative);
            }
            if (requestPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || requestPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return new Uri(requestPath);
            }
            return new Uri(baseAddress.TrimEnd('/') + "/" + requestPath.TrimStart('/'));

        }
        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestPath)
        {
            var uri = CombinUri(restInfo.BaseAddress, requestPath);
            var message = new HttpRequestMessage(method, uri);
            if (restInfo.DefaultHeaders != null)
            {
                foreach (var kv in restInfo.DefaultHeaders)
                {
                    message.Headers.Add(kv.Key, kv.Value);
                }
            }
            return message;
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
                var argument = apiInfo.Arguments.Single(p => p.Source == ArgumentSource.Router && nm.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
                return ValueToString(argument.Value);
            });
            return path2;
        }
        private static string ValueToString(object value)
        {
            if (value == null) return null;
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

        internal IDictionary<string, object> ObjectToMap(object obj)
        {
            if (obj == null) return null;
            if (obj is IDictionary<string, object>)
            {
                return obj as IDictionary<string, object>;
            }
            if (obj is IDictionary<string, string> strdic)
            {
                return strdic.ToDictionary(p => p.Key, p => p.Value as object);
            }
            return obj.GetType().GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p.GetValue(obj));
        }
        internal IEnumerable<KeyValuePair<string, string>> ObjectToStringKeyValuePairs(object obj)
        {
            if (obj == null) return Enumerable.Empty<KeyValuePair<string, string>>();

            return obj.GetType().GetProperties()
                 .Where(p => p.CanRead)
                 .SelectMany(p => GetPropertyValues(p, obj));
        }
        internal IEnumerable<KeyValuePair<string, string>> GetPropertyValues(PropertyInfo p, object obj)
        {
            var value = p.GetValue(obj);
            if ((Type.GetTypeCode(value.GetType()) == TypeCode.Object) && (value is IEnumerable valueList))
            {
                foreach (var item in valueList)
                {
                    //if (item != null)
                    {
                        yield return new KeyValuePair<string, string>(p.Name, ValueToString(item));
                    }
                }
            }
            else if (value != null)
            {
                yield return new KeyValuePair<string, string>(p.Name, ValueToString(value));
            }
        }

    }

}

