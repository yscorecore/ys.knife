using System;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public RestClient(RestInfo restInfo, HttpClient httpClient)
        {
            _ = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.restInfo = restInfo ?? new RestInfo();
            this.httpClient = httpClient;
        }

        #endregion

        #region public

        public Task SendHttp(ApiInfo apiInfo)
        {
            return SendHttpAsResponse(apiInfo);
        }
        public async Task<T> SendHttp<T>(ApiInfo apiInfo)
        {
            var response = await this.SendHttpAsResponse(apiInfo);
            return FromResponse<T>(response);
        }
        #endregion

        #region protected

        protected virtual async Task<HttpResponseMessage> SendHttpAsResponse(ApiInfo apiInfo)
        {
            _ = apiInfo ?? throw new ArgumentNullException(nameof(apiInfo));

            using (var request = new HttpRequestMessage())
            {
                request.Method = apiInfo.Method;
                request.RequestUri = BuildRequestUri(apiInfo);
                request.Content = apiInfo.Body;

                this.AppendAttributeHeaders(request);
                this.AppendRestInfoHeaders(request);
                this.AppendRequestHeaders(apiInfo, request);

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }

        protected virtual T FromResponse<T>(HttpResponseMessage response)
        {
            _ = response ?? throw new ArgumentNullException(nameof(response));
            var content = response.Content;

            var responseData = content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(responseData))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(responseData, JsonOptions);




        }

        private T ToObject<T>(JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        #endregion

        #region Header

        private void AppendAttributeHeaders(HttpRequestMessage message)
        {
            var headers = Attribute.GetCustomAttributes(this.GetType(), typeof(RequestHeaderAttribute), true)
                         .Cast<RequestHeaderAttribute>();
            foreach (var kv in headers)
            {
                if (!string.IsNullOrEmpty(kv.Key))
                {
                    message.Headers.Add(kv.Key, kv.Value);
                }

            }
        }
        private void AppendRestInfoHeaders(HttpRequestMessage message)
        {
            if (restInfo.Headers != null)
            {
                foreach (var kv in restInfo.Headers)
                {
                    if (!string.IsNullOrEmpty(kv.Key))
                    {
                        message.Headers.Add(kv.Key, kv.Value);
                    }

                }
            }
        }
        private void AppendRequestHeaders(ApiInfo apiInfo, HttpRequestMessage httpRequestMessage)
        {
            if (apiInfo.Headers != null)
            {
                foreach (var kv in apiInfo.Headers)
                {
                    if (!string.IsNullOrEmpty(kv.Key))
                    {
                        httpRequestMessage.Headers.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        #endregion

        #region Url
        private Uri BuildRequestUri(ApiInfo apiInfo)
        {
            var requestPath = GetTranslatedPath(apiInfo);
            var queryString = BuildQueryString(apiInfo);
            var requestPathWithQuery = CombinQueryString(requestPath, queryString);
            return CombinUri(requestPathWithQuery);
        }
        private string GetTranslatedPath(ApiInfo apiInfo)
        {
            // replace {key},{key:int},{age:range(18,120)},{ssn:regex(^\d{{3}}-\d{{2}}-\d{{4}}$)}
            if (apiInfo.Route == null) return apiInfo.Path;
            return Regex.Replace(apiInfo.Path ?? string.Empty, "\\{(?<nm>\\w+)(\\?)?(:.+)?\\}", (m) =>
              {
                  var nm = m.Groups["nm"].Value;
                  if (apiInfo.Route.TryGetValue(nm, out string value))
                  {
                      return UrlEncoder.Default.Encode(value);
                  }
                  else
                  {
                      return m.Value;
                  }
              });
        }

        private string BuildQueryString(ApiInfo apiInfo)
        {
            if (apiInfo.Query == null) return string.Empty;
            return string.Join("&", apiInfo.Query.Where(p => !string.IsNullOrEmpty(p.Key))
                .Select(kv => $"{UrlEncoder.Default.Encode(kv.Key)}={UrlEncoder.Default.Encode(kv.Value ?? string.Empty)}"));
        }
        private string CombinQueryString(string requestPath, string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                if (requestPath.Contains("?"))
                {

                    return requestPath + query;
                }
                else
                {
                    return requestPath + "?" + query;
                }
            }
            return requestPath;
        }

        private Uri CombinUri(string requestPath)
        {
            if (requestPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || requestPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return new Uri(requestPath);
            }
            string baseAddress = restInfo.BaseAddress;
            if (string.IsNullOrEmpty(baseAddress))
            {
                var attr = Attribute.GetCustomAttribute(this.GetType(), typeof(RestClientAttribute)) as RestClientAttribute;
                if (attr != null)
                {
                    baseAddress = attr.DefaultBaseAddress;
                }
            }
            if (string.IsNullOrEmpty(baseAddress))
            {
                return new Uri(requestPath, UriKind.Relative);
            }
            return new Uri(baseAddress.TrimEnd('/') + "/" + requestPath.TrimStart('/'));
        }
        #endregion


    }

}

