using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCFunctions.Models
{
    
    
        public class CustomerDto
        {
            public string customerId { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string surname { get; set; } = string.Empty;
            public string username { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string shippingAddress { get; set; } = string.Empty;
        }

        public class ProductDto
        {
            public string productId { get; set; } = string.Empty;
            public string productName { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public decimal price { get; set; }
            public int stockAvailable { get; set; }
            public string imageUrl { get; set; } = string.Empty;
        }

        public class OrderDto
        {
            public string orderId { get; set; } = string.Empty;
            public string customerId { get; set; } = string.Empty;
            public string username { get; set; } = string.Empty;
            public string productId { get; set; } = string.Empty;
            public string productName { get; set; } = string.Empty;
            public DateTime orderDate { get; set; }
            public int quantity { get; set; }
            public decimal unitPrice { get; set; }
            public decimal totalPrice { get; set; }
            public string status { get; set; } = "Pending";
        }

        public class CreateOrderRequest
        {
            public string customerId { get; set; } = string.Empty;
            public string productId { get; set; } = string.Empty;
            public int quantity { get; set; }
        }

        public class UploadResponse
        {
            public string fileUrl { get; set; } = string.Empty;
            public string fileName { get; set; } = string.Empty;
            public long fileSize { get; set; }
        }
    
}
