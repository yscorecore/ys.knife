using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Hosting.Web
{
    [Options("Knife")]
    public class KnifeWebOptions : KnifeOptions
    {
        public Dictionary<string, StaticFileInfo> StaticFiles { get; set; } = new Dictionary<string, StaticFileInfo>();

        public bool WrapCodeMessageResult { get; set; } = false;
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
