using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IA03.Controllers
{
    [Route("health")]
    public class Health : Controller
    {
        private readonly ILogger<Health> _logger;

        public Health(ILogger<Health> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            _logger.LogInformation("Health check accessed.");
            return Ok("Healthy");
        }
    }
}