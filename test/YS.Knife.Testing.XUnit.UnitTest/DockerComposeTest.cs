using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Testing.XUnit.UnitTest
{
    public class DockerComposeTest
    {
        [Fact]
        public void TestUp()
        {
            DockerCompose.Up("docker-compose.yml", null, new Dictionary<string, int> { ["mongo"] = 27017 });
        }
    }
}
