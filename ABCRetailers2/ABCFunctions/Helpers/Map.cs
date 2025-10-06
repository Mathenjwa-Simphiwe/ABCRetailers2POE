using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCFunctions.Entities;
using ABCFunctions.Models;

namespace ABCFunctions.Helpers
{
    public static class Map
    {
        // Entity to DTO
        public static CustomerDto ToDto(this CustomerEntity entity) => new()
        {
            customerId = entity.CustomerId,
            name = entity.Name,
            surname = entity.Surname,
            username = entity.Username,
            email = entity.Email,
            shippingAddress = entity.ShippingAddress
        };

        public static ProductDto ToDto(this ProductEntity entity) => new()
        {
            productId = entity.ProductId,
            productName = entity.ProductName,
            description = entity.Description,
            price = entity.Price,
            stockAvailable = entity.StockAvailable,
            imageUrl = entity.ImageUrl
        };

        public static OrderDto ToDto(this OrderEntity entity) => new()
        {
            orderId = entity.OrderId,
            customerId = entity.CustomerId,
            username = entity.Username,
            productId = entity.ProductId,
            productName = entity.ProductName,
            orderDate = entity.OrderDate,
            quantity = entity.Quantity,
            unitPrice = entity.UnitPrice,
            totalPrice = entity.TotalPrice,
            status = entity.Status
        };

        // DTO to Entity
        public static CustomerEntity ToEntity(this CustomerDto dto) => new()
        {
            CustomerId = dto.customerId,
            Name = dto.name,
            Surname = dto.surname,
            Username = dto.username,
            Email = dto.email,
            ShippingAddress = dto.shippingAddress
        };

        public static ProductEntity ToEntity(this ProductDto dto) => new()
        {
            ProductId = dto.productId,
            ProductName = dto.productName,
            Description = dto.description,
            Price = dto.price,
            StockAvailable = dto.stockAvailable,
            ImageUrl = dto.imageUrl
        };
    }
}
