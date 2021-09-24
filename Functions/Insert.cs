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
    public static class Insert
    {
        [FunctionName("Insert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string who = data?.who;
            DateTime when = data?.when;
            string what = data?.what;

            dynamic currentEntityData = data?.currentEntity;
            Entity currentEntity = new Entity{
                Id = currentEntityData?.id,
                Name = currentEntityData?.name,
                DataType = currentEntityData?.dataType,
                Value = currentEntityData?.value
            };

            dynamic oldEntityData = data?.oldEntity;
            Entity oldEntity = oldEntityData == null ? null : new Entity{
                Id = oldEntityData?.id,
                Name = oldEntityData?.name,
                DataType = oldEntityData?.dataType,
                Value = oldEntityData?.value
            };

            string ip = data?.ip;
            string computerName = data?.computerName;
            string ticketNumber = data?.ticketNumber;

            EntryHelper entryHelper = new EntryHelper(log);
            Entry entry = new Entry() 
            { 
                Who = who, 
                When = when, 
                What = what,
                CurrentEntity = currentEntity,
                OldEntity = oldEntity,
                Ip = ip,
                ComputerName = computerName,
                TicketNumber = ticketNumber
            };
            await entryHelper.AddItemsToContainerAsync(entry);

            bool result = true;

            string responseMessage = result
                ? "success"
                : "failure";

            return new OkObjectResult(responseMessage);
        }
    }
}
