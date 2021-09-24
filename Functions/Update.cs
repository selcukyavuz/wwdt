using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class Update
    {
        [FunctionName("Update")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string id = data?.id;
            string who = data?.who;
            DateTime when = data?.when;
            string what = data?.what;

            EntryHelper entryHelper = new EntryHelper(log);
            Entry entry = new Entry() 
            { 
                Who = who, 
                When = when, 
                What = what 
            };

            await entryHelper.ReplaceEntryItemAsync(id,entry);

            bool result = true;

            string responseMessage = result
                ? "success"
                : "failure";

            return new OkObjectResult(responseMessage);
        }
    }
}
