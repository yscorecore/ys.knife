using System;

namespace Knife
{
    [OptionsClass()]
    public class AppOptions
    {
        public string DbType { get; set; } = "mysql";

        public string DataFolder { get; set; } = "app_data";

        public string LogFolder { get; set; } = "app_log";

        public string TempFolder { get; set; } = "app_temp";

        public string[] Plugins { get; set; } = new[] { "*.dll" };

    }
}
