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
    public static class SelectById
    {
        [FunctionName("SelectById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get" , Route = null)] HttpRequest req,
            ILogger log)
        {
            string EndPointUri = Environment.GetEnvironmentVariable("EndpointUri");

            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];
            
            EntryHelper entryHelper = new EntryHelper(log);
            
            Entry entry = await entryHelper.QueryItemByIdAsync(id);

            return new OkObjectResult(entry);
        }
    }
}
