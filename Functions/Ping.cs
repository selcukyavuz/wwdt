using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Company.Function
{
    public static class Ping
    {
        [FunctionName("Ping")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get" , Route = null)] HttpRequest req,
            ILogger log)
        {
            return await Task.Run(() => new OkObjectResult("OK")) ;
        }
    }
}
