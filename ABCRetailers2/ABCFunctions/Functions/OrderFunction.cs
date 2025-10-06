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
using Azure.Storage.Blobs;

namespace ABCFunctions.Functions
{
    public class OrdersFunction
    {
        private readonly string _conn;
        private readonly string _table;

        public OrdersFunction(IConfiguration cfg)
        {
            _conn = cfg["STORAGE_CONNECTION"] ?? throw new InvalidOperationException("STORAGE_CONNECTION missing");
            _table = cfg["TABLE_ORDER"] ?? "Orders";
        }

        [Function("Orders_List")]
        public async Task<HttpResponseData> List(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders")] HttpRequestData req)
        {
            var table = new TableClient(_conn, _table);
            await table.CreateIfNotExistsAsync();

            var items = new List<OrderDto>();
            await foreach (var e in table.QueryAsync<OrderEntity>(x => x.PartitionKey == "Order"))
                items.Add(Map.ToDto(e));

            return HttpJson.Ok(req, items);
        }

        [Function("Orders_Get")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id}")] HttpRequestData req, string id)
        {
            var table = new TableClient(_conn, _table);
            try
            {
                var e = await table.GetEntityAsync<OrderEntity>("Order", id);
                return HttpJson.Ok(req, Map.ToDto(e.Value));
            }
            catch
            {
                return HttpJson.NotFound(req, "Order not found");
            }
        }

        public record OrderCreateRequest(string CustomerId, string ProductId, int Quantity, decimal UnitPrice);

        [Function("Orders_Create")]
        public async Task<HttpResponseData> Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req)
        {
            var input = await HttpJson.ReadAsync<OrderCreateRequest>(req);
            if (input is null || string.IsNullOrWhiteSpace(input.CustomerId) || string.IsNullOrWhiteSpace(input.ProductId))
                return HttpJson.BadRequest(req, "CustomerId and ProductId are required");

            var table = new TableClient(_conn, _table);
            await table.CreateIfNotExistsAsync();

            var e = new OrderEntity
            {
                PartitionKey = "Order",
                RowKey = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = input.CustomerId,
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                UnitPrice = input.UnitPrice,
                TotalPrice = input.UnitPrice * input.Quantity,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };
            await table.AddEntityAsync(e);

            return HttpJson.Created(req, Map.ToDto(e));
        }

        [Function("Orders_UpdateStatus")]
        public async Task<HttpResponseData> UpdateStatus(
     [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "orders/{id}/status")] HttpRequestData req, string id)
        {
            var statusRequest = await HttpJson.ReadAsync<StatusUpdateRequest>(req);
            if (statusRequest == null || string.IsNullOrWhiteSpace(statusRequest.Status))
                return HttpJson.BadRequest(req, "Status is required");

            var table = new TableClient(_conn, _table);
            try
            {
                var resp = await table.GetEntityAsync<OrderEntity>("Order", id);
                var e = resp.Value;
                e.Status = statusRequest.Status;  // ← FIXED: Use statusRequest.Status
                await table.UpdateEntityAsync(e, e.ETag, TableUpdateMode.Replace);
                return HttpJson.Ok(req, Map.ToDto(e));
            }
            catch
            {
                return HttpJson.NotFound(req, "Order not found");
            }
        }

        // ADD THIS INSIDE THE CLASS:
        public record StatusUpdateRequest(string Status);

        [Function("Orders_Delete")]
        public async Task<HttpResponseData> Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "orders/{id}")] HttpRequestData req, string id)
        {
            var table = new TableClient(_conn, _table);
            await table.DeleteEntityAsync("Order", id);
            return HttpJson.NoContent(req);
        }
    }
}