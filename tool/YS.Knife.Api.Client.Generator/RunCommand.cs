using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace YS.Knife.Rest.Api.Client.Generator
{
    [SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
    public class RunCommand
    {
        public void Run(ApiOptions apiOptions)
        {

        }

        public string BuildProject(ApiOptions apiOptions, string targetFolder)
        {
            System.IO.Directory.CreateDirectory(targetFolder);
            Shell.Exec("dotnet", $"build {apiOptions.ProjectPath} -o {targetFolder}", true);
            return null;

        }
        public void GeneratorCsproj(ApiOptions apiOptions)
        {

        }


        public void GeneratorClientFiles()
        {

        }

    }
}
