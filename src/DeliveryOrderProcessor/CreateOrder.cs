using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryOrderProcessor
{
    public static class CreateOrder
    {
        [FunctionName("CreateOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "orders-db",
                containerName: "orders-container",
                Connection = "CosmosDbConnectionString")]IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (!string.IsNullOrEmpty(requestBody))
            {
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                // Add a JSON document to the output container.
                await documentsOut.AddAsync(new
                {
                    // create a random ID
                    id = Guid.NewGuid().ToString(),
                    buyerId = data.BuyerId,
                    orderDate = data.OrderDate,
                    shippingAddress = $"{data.ShipToAddress.Street}, {data.ShipToAddress.City}, {data.ShipToAddress.State}, {data.ShipToAddress.Country}, {data.ShipToAddress.ZipCode}",
                    orderItems = string.Join(", ", ((IEnumerable<dynamic>)data.OrderItems).Select(i => (string)i.ItemOrdered.ProductName).ToList()),
                    totalPrice = data.TotalPrice
                });
            }

            return new OkObjectResult("This HTTP triggered function executed successfully.");
        }
    }
}
