using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Threading;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.AspNetCore.Http;

namespace OrderItemsReserver
{
    public static class CreateOrder
    {           
        /*[FunctionName(nameof(CreateOrder))]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureAccountStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var fileName = $"Order_{DateTime.UtcNow.ToString("F")}.json";
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(fileName);
            await blob.UploadAsync(req.Body);
            return new OkObjectResult("file uploaded successfylly");
        }*/

        [FunctionName("PostOrder")]
        public static async Task RunAsync(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")] string myQueueItem,
            CancellationToken token,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var fileName = $"Order_{DateTime.UtcNow.ToString("F")}.json";
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(fileName);
            await blob.UploadAsync(new BinaryData(myQueueItem), token);
        }
    }
}
