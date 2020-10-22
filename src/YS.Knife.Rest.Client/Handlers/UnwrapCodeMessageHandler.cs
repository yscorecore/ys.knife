using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Client.Handlers
{
    public class UnwrapCodeMessageHandler : DelegatingHandler
    {
        public UnwrapCodeMessageHandler(UnwrapCodeMessageOptions unwrapCodeMessageOptions)
        {
            this.unwrapCodeMessageOptions = unwrapCodeMessageOptions;
        }
        private readonly UnwrapCodeMessageOptions unwrapCodeMessageOptions;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = await base.SendAsync(request, cancellationToken);
            if (isJson(message.Content))
            {
                await TryUnwrapCodeMessage(message);
            }


            return message;
        }
        private bool isJson(HttpContent content)
        {
            return content.Headers.ContentType.MediaType == "application/json";
        }
        private async Task TryUnwrapCodeMessage(HttpResponseMessage message)
        {
            string text = await message.Content.ReadAsStringAsync();

            var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(text);
            var code = values[unwrapCodeMessageOptions.CodeProperty].GetString();
            if (code == unwrapCodeMessageOptions.SuccessCodeValue)
            {
                if (values.TryGetValue(unwrapCodeMessageOptions.ResultProperty, out var res))
                {
                    message.Content = new StringContent(res.GetRawText(), Encoding.UTF8);
                }
                else
                {
                    message.Content = new StringContent(string.Empty, Encoding.UTF8);
                }
            }
            else
            {
                string errorMessage = values[unwrapCodeMessageOptions.MessageProperty].GetString();
                throw new KnifeException(code, errorMessage);
            }
            //JsonDocument document = JsonDocument.Parse(text);


            //JsonElement codeElement = document.RootElement.GetProperty(unwrapCodeMessageOptions.CodeProperty);

            //if (codeElement.GetString() != unwrapCodeMessageOptions.SuccessCodeValue)
            //{
            //    JsonElement messageElement = document.RootElement.GetProperty(unwrapCodeMessageOptions.MessageProperty);

            //    throw new CodeException(codeElement.GetString(), messageElement.GetString());
            //}
            //else
            //{
            //    JsonElement result = document.RootElement.GetProperty(unwrapCodeMessageOptions.ResultProperty);
            //    message.Content = new JsonContent(JsonSerializer.Serialize(result));

            //}


        }
    }

    [Options]
    public class UnwrapCodeMessageOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string CodeProperty { get; set; } = "code";

        [Required(AllowEmptyStrings = false)]
        public string MessageProperty { get; set; } = "message";

        [Required(AllowEmptyStrings = false)]
        public string ResultProperty { get; set; } = "result";

        public string SuccessCodeValue { get; set; } = "0";
    }
}
