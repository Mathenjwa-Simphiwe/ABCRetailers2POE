using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace ABCFunctions.Helpers
{
    public static class HttpJson
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static async Task<T> ReadAsync<T>(HttpRequestData req)
        {
            using var reader = new StreamReader(req.Body);
            var content = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(content, _options);
        }

        public static HttpResponseData Ok<T>(HttpRequestData req, T data)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(data, _options));
            return response;
        }

        public static HttpResponseData Created<T>(HttpRequestData req, T data)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.Created);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(data, _options));
            return response;
        }

        public static HttpResponseData BadRequest(HttpRequestData req, string message)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(new { error = message }, _options));
            return response;
        }

        public static HttpResponseData NotFound(HttpRequestData req, string message)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString(JsonSerializer.Serialize(new { error = message }, _options));
            return response;
        }

        public static HttpResponseData NoContent(HttpRequestData req)
        {
            return req.CreateResponse(System.Net.HttpStatusCode.NoContent);
        }
    }
}