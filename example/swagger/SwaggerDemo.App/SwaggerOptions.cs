using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SwaggerDemo.App
{
    [YS.Knife.OptionsClass]
    public class SwaggerOptions
    {
        public bool Enabled { get; set; }
        public string DocumentName { get; set; }

        public ApiInfo Api { get; set; } = new ApiInfo();

        public UIInfo UI { get; set; } = new UIInfo();
    }

    public class ApiInfo
    {
        public string Version { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 是否包含xml文档（有助于swagger的描述说明）
        /// </summary>
        public bool IncludeXmlComments { get; set; } = true;
        /// <summary>
        /// xml文档的搜索通配符
        /// </summary>
        public string XmlComentsFiles { get; set; } = "*.xml";

        /// <summary>
        /// 表示swagger文档的请求地址模板
        /// </summary>
        public string RouteTemplate { get; set; } = "/swagger/{documentName}/swagger.json";
    }
    public class UIInfo
    {
        /// <summary>
        /// UI的路径前缀
        /// </summary>
        /// <remarks>the swagger ui request url will be {UIRoutePrefix}/index.html </remarks>
        public string RoutePrefix { get; set; } = "swagger";
    }

    public static class SwaggerOptionsExtensions
    {
        public static string GetDocumentNameOrEntryAssemblyName(this SwaggerOptions swaggerOptions)
        {
            if (string.IsNullOrEmpty(swaggerOptions.DocumentName))
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
            else
            {
                return swaggerOptions.DocumentName;

            }
        }
        public static string GetTitleOrAssemblyTitle(this ApiInfo apiInfo)
        {
            if (string.IsNullOrEmpty(apiInfo.Title))
            {
                var attr = Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
                if (attr != null)
                {
                    return attr.Title;
                }
            }
            return apiInfo.Title;
        }
        public static string GetDescriptionOrAssemblyDescription(this ApiInfo apiInfo)
        {
            if (string.IsNullOrEmpty(apiInfo.Description))
            {
                var attr = Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
                if (attr != null)
                {
                    return attr.Description;
                }
            }
            return apiInfo.Description;
        }
        public static string GetVersionOrAssemblyVersion(this ApiInfo apiInfo)
        {
            if (string.IsNullOrEmpty(apiInfo.Version))
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            }
            return apiInfo.Version;
        }
    }

}
