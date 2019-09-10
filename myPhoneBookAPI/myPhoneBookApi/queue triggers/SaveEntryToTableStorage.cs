using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace myPhoneBookApi
{
    public static class SaveEntryToTableStorage
    {
        [FunctionName("SaveEntryToTableStorage")]
        [return: Table("PhoneBook")]
        public static entry TableOutput([QueueTrigger("newentryqueue", Connection = "NewEntryQueue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger: {myQueueItem}");
            var entry = JsonConvert.DeserializeObject<entry>(myQueueItem);
            return new entry { PartitionKey = "Http", RowKey = Guid.NewGuid().ToString(), name = entry.name, phone = entry.phone };
        }
    }


}
