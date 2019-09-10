using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace myPhoneBookApi
{
    public static class addentry
    {
        [FunctionName("addentry")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("newentryqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<entry> msg,
            ILogger log)
        {
            log.LogInformation("C# Http Trigger: addentry");

            if (req.Body.CanSeek) req.Body.Position = 0;
            var input = new StreamReader(req.Body).ReadToEnd();
            log.LogInformation("addentry - data: " + input.ToString());
            var entry = JsonConvert.DeserializeObject<entry>(input);

            if (entry != null)
            {
                msg.Add(entry);
                return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(entry));
            }

            return new BadRequestObjectResult("Please pass an entry object.");
        }
    }
}
