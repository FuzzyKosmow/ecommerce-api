using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.Models;

namespace ecommerce_api.Services.OrderService
{
    public interface IOrderService
    {
        Task<Models.Order> CreateOrderAsync(DTO.Order.CreateOrderDTO order);
        Task<Models.Order> GetOrderAsync(int orderId);
        Task<List<Order>> GetOrdersAsync(int page, int limit);
        Task<List<Order>> GetOrdersFilter(string status, string paymentMethod, string userId, int page, int limit);
        Task<Models.Order> UpdateOrderAsync(int orderId, DTO.Order.UpdateOrderDTO order);
        Task<bool> DeleteOrderAsync(int orderId);



        Task<List<Order>?> GetOrderFromCustomer(string customerId, int page, int limit);
        Task<List<Order>?> GetOrderFromCustomerEmail(string email, int page, int limit);
        Task<decimal> TotalValueFromCustomer(string customerId);
    }
}