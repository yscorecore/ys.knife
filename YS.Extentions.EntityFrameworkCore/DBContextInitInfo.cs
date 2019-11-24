using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public class DBContextInitInfo
    {
        public string DbType { get; set; }
        public Type ContextType { get; set; }
        public string ConnectionString { get; set; }
        public string MigrationAssemblyTemplate { get; set; }
        public string ContextName { get; set; }
        public string ContextFullName { get; set; }
        public string AssemblyName { get; set; }
        public IConfiguration ConfigNode { get; set; }


        public string GetMigrationAssemblyName()
        {
            if (string.IsNullOrEmpty(this.MigrationAssemblyTemplate)) return this.MigrationAssemblyTemplate;

            return Regex.Replace(this.MigrationAssemblyTemplate, @"{(?<key>\w+)}", this.ReplaceVar,RegexOptions.IgnoreCase);
        }
        private string ReplaceVar(Match match)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "dbtype",this.DbType},
                { "contextname",this.ContextName},
                { "contextfullname",this.ContextFullName},
                { "assemblyname",this.AssemblyName}
            };
            var key = match.Groups["key"].Value;
            if (vars.ContainsKey(key))
            {
                return vars[key];
            }
            return match.Value;
        }
    }
}
