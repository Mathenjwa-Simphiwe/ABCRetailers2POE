using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using Azure.Storage.Blobs;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Azure.WebJobs;

namespace ABCFunctions.Functions
{
    public class BlobFunction
    {
        [Function("OnProductImageUploaded")]
        public void OnProductImageUploaded(
            [BlobTrigger("%BLOB_PRODUCT_IMAGES%/{name}", Connection = "STORAGE_CONNECTION")] Stream blob,
            string name,
            FunctionContext ctx)
        {
            var log = ctx.GetLogger("OnProductImageUploaded");
            log.LogInformation($"Product image uploaded: {name}, size={blob.Length} bytes");
        }
    }
}
