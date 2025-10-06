using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ABCFunctions.Entities;
using ABCFunctions.Helpers;
using ABCFunctions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;

namespace ABCFunctions.Functions
{
    public class ProductsFunction
    {
        private readonly string _conn;
        private readonly string _table;

        public ProductsFunction(IConfiguration cfg)
        {
            _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
            _table = cfg["TABLE_PRODUCT"] ?? "Products";
        }

        [Function("Products_List")]
        public async Task<HttpResponseData> List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequestData req)
        {
            var table = new TableClient(_conn, _table);
            await table.CreateIfNotExistsAsync();

            var items = new List<ProductDto>();
            await foreach (var e in table.QueryAsync<ProductEntity>(x => x.PartitionKey == "Product"))
                items.Add(Map.ToDto(e));

            return HttpJson.Ok(req, items);
        }

        [Function("Products_Get")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequestData req, string id)
        {
            var table = new TableClient(_conn, _table);
            try
            {
                var e = await table.GetEntityAsync<ProductEntity>("Product", id);
                return HttpJson.Ok(req, Map.ToDto(e.Value));
            }
            catch
            {
                return HttpJson.NotFound(req, "Product not found");
            }
        }

        public record ProductCreateRequest(string ProductName, string Description, decimal Price, int StockAvailable, string ImageUrl);

        [Function("Products_Create")]
        public async Task<HttpResponseData> Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequestData req)
        {
            var input = await HttpJson.ReadAsync<ProductCreateRequest>(req);
            if (input is null || string.IsNullOrWhiteSpace(input.ProductName))
                return HttpJson.BadRequest(req, "ProductName is required");

            var table = new TableClient(_conn, _table);
            await table.CreateIfNotExistsAsync();

            var e = new ProductEntity
            {
                PartitionKey = "Product",
                RowKey = Guid.NewGuid().ToString(),
                ProductId = Guid.NewGuid().ToString(),
                ProductName = input.ProductName,
                Description = input.Description ?? "",
                Price = input.Price,
                StockAvailable = input.StockAvailable,
                ImageUrl = input.ImageUrl ?? ""
            };
            await table.AddEntityAsync(e);

            return HttpJson.Created(req, Map.ToDto(e));
        }

        [Function("Products_Update")]
        public async Task<HttpResponseData> Update(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequestData req, string id)
        {
            var input = await HttpJson.ReadAsync<ProductCreateRequest>(req);
            if (input is null) return HttpJson.BadRequest(req, "Invalid body");

            var table = new TableClient(_conn, _table);
            try
            {
                var resp = await table.GetEntityAsync<ProductEntity>("Product", id);
                var e = resp.Value;

                e.ProductName = input.ProductName ?? e.ProductName;
                e.Description = input.Description ?? e.Description;
                e.Price = input.Price;
                e.StockAvailable = input.StockAvailable;
                e.ImageUrl = input.ImageUrl ?? e.ImageUrl;

                await table.UpdateEntityAsync(e, e.ETag, TableUpdateMode.Replace);
                return HttpJson.Ok(req, Map.ToDto(e));
            }
            catch
            {
                return HttpJson.NotFound(req, "Product not found");
            }
        }

        [Function("Products_Delete")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequestData req, string id)
        {
            var table = new TableClient(_conn, _table);
            await table.DeleteEntityAsync("Product", id);
            return HttpJson.NoContent(req);
        }
    }
}