using System.Text.RegularExpressions;
using YS.Knife;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public const string DBTypeKey = "@DbType";
        public const string DefaultDbType = "mysql";
        public static string GetDbType(this IConfiguration configuration)
        {
            var dbtype = configuration.GetConnectionString(DBTypeKey);
            return string.IsNullOrEmpty(dbtype) ? DefaultDbType : dbtype;
        }

        public static ConnectionInfo GetConnectionInfo(this IConfiguration configuration, string name)
        {
            var connectionString = configuration.GetConnectionString(name);
            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            var match = Regex.Match(connectionString, "^(?<type>\\w+)#(?<conn>.+)$");
            if (match.Success)
            {
                return new ConnectionInfo
                {
                    DBType = match.Groups["type"].Value,
                    Value = match.Groups["conn"].Value,
                };
            }
            else
            {
                return new ConnectionInfo
                {
                    DBType = configuration.GetDbType(),
                    Value = connectionString,
                };
            }

        }
    }
}
