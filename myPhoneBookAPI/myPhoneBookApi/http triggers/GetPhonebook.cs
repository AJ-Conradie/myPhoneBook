using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace myPhoneBookApi
{
    public static class GetPhonebook
    {
        [FunctionName("phonebook")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
           [Table("PhoneBook")] CloudTable cloudTable,
           ILogger log)
        {
            log.LogInformation($"C# Http Trigger: GetPhonebook, executed at: {DateTime.Now}");

            TableQuery<entry> rangeQuery = new TableQuery<entry>();//.Where();

            phonebook phoneBook = new phonebook();
            phoneBook.name = "Server";
            foreach (entry entity in
                await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
            {
                log.LogInformation(
                    $"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.name}");
                phoneBook.entries.Add(new entry() { name = entity.name, phone = entity.phone });
            }

            return new OkObjectResult(phoneBook);
        }

    }
}
