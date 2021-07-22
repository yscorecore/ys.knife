using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Hosting.Web
{
    [Options("Knife")]
    public class KnifeWebOptions : KnifeOptions
    {
        public Dictionary<string, StaticFileInfo> StaticFiles { get; set; } = new Dictionary<string, StaticFileInfo>();

        public bool WrapCodeException { get; set; } = false;

        public int? DefaultErrorStatusCode { get; set; } = 900;

        public List<CodeExceptionMap> CodeExceptionMap { get; set; } = new List<CodeExceptionMap>();

    }
    public class CodeExceptionMap
    {
        public string Code { get; set; }
        public string ExceptionPattern { get; set; }
        public int? StatusCode { get; set; }
    }


    public class StaticFileInfo
    {
        public bool EnableDirectoryBrowsing { get; set; } = false;
        public bool ServeUnknownFileTypes { get; set; } = false;
        [Required(AllowEmptyStrings = false)]
        public string FolderPath { get; set; }
        public string DefaultContentType { get; set; }
    }
}


