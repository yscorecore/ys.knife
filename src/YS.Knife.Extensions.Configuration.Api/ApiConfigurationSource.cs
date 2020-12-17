using System;
using Microsoft.Extensions.Configuration;

namespace YS.Knife.Extensions.Configuration.Api
{
    public class ApiConfigurationSource : IConfigurationSource
    {
        /// <summary>  
        /// Specifies the url of RESTful API.  
        /// </summary>  
        public string ApiUrl { get; set; }

        /// <summary>  
        /// Specifies the polling period.  
        /// </summary>  
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);

        public TimeSpan ReloadAfter { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>  
        /// Specifies whether this source is optional.  
        /// </summary>  
        public bool Optional { get; set; }



        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {

            return new ApiConfigurationProvider(this);
        }
    }
}
