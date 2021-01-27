
using System.ComponentModel.DataAnnotations;

namespace YS.Knife.Hosting.Web.Health
{
    [Options("Knife.Health")]
    public class HealthOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string RequestPath { get; set; } = "/health";
        public bool CachingResponse { get; set; } = false;
        [Range(1, 99999)]
        public int UnhealthyCode { get; set; } = 503;
        [Range(1, 99999)]
        public int DegradedCode { get; set; } = 200;
        [Range(1, 99999)]
        public int HealthyCode { get; set; } = 200;
    }
}
