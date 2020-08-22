using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YS.Knife.Rest.Client.TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestgetController : ControllerBase
    {
        // GET: api/<TestgetController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [HttpGet]
        [Route("{id}")]
        public IEnumerable<string> Get(string id, [FromQuery] string arg1, [FromHeader] string arg2)
        {
            return new string[] { id, arg1, arg2 }.Where(p => !string.IsNullOrEmpty(p));
        }

    }
}
