using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ABCFunctions.Helpers;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
namespace ABCFunctions.Functions
{
    public class UploadFunctions
    {
        private readonly string _conn;
        private readonly string _proofs;
        private readonly string _share;
        private readonly string _shareDir;

        public UploadFunctions(IConfiguration cfg)
        {
            _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
            _proofs = cfg["BLOB_PAYMENT_PROOFS"] ?? "payment-proofs";
            _share = cfg["FILESHARE_CONTRACTS"] ?? "contracts";
            _shareDir = cfg["FILESHARE_DIR_PAYMENTS"] ?? "payments";
        }

        [Function("Uploads_ProofOfPayment")]
        public async Task<HttpResponseData> Proof(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploads/proof-of-payment")] HttpRequestData req)
        {
            try
            {
                // Check content type
                if (!req.Headers.TryGetValues("Content-Type", out var contentTypes) ||
                    !contentTypes.Any(ct => ct.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase)))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Expected multipart/form-data");
                    return badResponse;
                }

                // Read multipart form data manually
                var body = await new StreamReader(req.Body).ReadToEndAsync();

                // For now, return a placeholder response
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync("File upload endpoint - multipart parsing needs to be implemented");
                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }

    }
}
