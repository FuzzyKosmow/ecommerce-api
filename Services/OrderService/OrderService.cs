using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Order;
using ecommerce_api.Models;
using ecommerce_api.Services.ProductService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Services.OrderService
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IProductService productService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _productService = productService;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<Order> CreateOrderAsync(CreateOrderDTO order)
        {
            // Create a new order
            Order newOrder = new Order
            {
                UserId = order.CustomerId,
                PaymentMethod = order.PaymentMethod,
                Status = "Pending",
                OrderDate = DateTime.Now
            };

            newOrder.OrderDetails = new List<OrderDetail>();
            // Add order details
            foreach (var orderDetail in order.OrderDetails)
            {
                Product? product = await _productService.GetProduct(orderDetail.ProductId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }
                newOrder.OrderDetails.Add(new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Quantity = orderDetail.Quantity,
                    Price = (decimal)(product.DiscountPrice != null && product.DiscountPrice > 0 ? product.DiscountPrice : product.Price)

                });

            }

            // Add the order to the database
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;

        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetOrderAsync(int orderId)
        {
            // Include the order details
            return await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<Order>> GetOrdersAsync(int page, int limit)
        {

            return _context.Orders.Skip((page - 1) * limit).Take(limit).ToList();
        }


        public async Task<List<Order>> GetOrdersFilter(string status, string paymentMethod, string userId, int page, int limit)
        {
            return await _context.Orders.Include(o => o.OrderDetails).Where(o => (status == null || o.Status == status) && (paymentMethod == null || o.PaymentMethod == paymentMethod) && (userId == null || o.UserId == userId)).Skip((page - 1) * limit).Take(limit).ToListAsync();
        }

        public async Task<Order?> UpdateOrderAsync(int orderId, UpdateOrderDTO order)
        {
            Order? orderModel = await _context.Orders.FindAsync(orderId);
            if (orderModel == null)
            {
                return null;
            }

            _mapper.Map(order, orderModel);
            _context.SaveChanges();
            return orderModel;


        }

        public async Task<List<Order>?> GetOrderFromCustomer(string id, int page = 1, int limit = 10)

        {

            return await _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).Where(o => o.UserId == id).Skip((page - 1) * limit).Take(limit).ToListAsync();
        }

        public async Task<List<Order>?> GetOrderFromCustomerEmail(string email, int page, int limit)
        {

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }
            return await GetOrderFromCustomer(user.Id, page, limit);
        }
    }
}