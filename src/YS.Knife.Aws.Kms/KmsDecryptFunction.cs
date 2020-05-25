using System.Collections.Generic;
using Amazon.KeyManagementService;
using System;
using System.Text;

namespace YS.Knife.Options.Functions
{
    [OptionsPostFunction]
    [DictionaryKey("kmsdecrypt")]
    public class KmsDecryptFunction : IOptionsPostFunction
    {
        public KmsDecryptFunction(IAmazonKeyManagementService keyManagementService)
        {
            this.keyManagementService = keyManagementService;
        }
        private readonly IAmazonKeyManagementService keyManagementService;
        public string Invoke(string args)
        {
            if (string.IsNullOrEmpty(args)) return args;
            var bytes = Convert.FromBase64String(args);
            var results = keyManagementService.Decrypt(bytes, new Dictionary<string, string>());
            return Encoding.UTF8.GetString(results);
        }
    }
}
