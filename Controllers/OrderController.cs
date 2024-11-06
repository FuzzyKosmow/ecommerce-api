using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Order;
using ecommerce_api.Models;
using ecommerce_api.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public readonly string ORDER_STATUS_PENDING = "PENDING";
        public readonly string ORDER_STATUS_COMPLETED = "DELIVERING";
        public readonly string ORDER_STATUS_CANCELLED = "CANCELLED";
        public readonly string ORDER_STATUS_DELIVERED = "DELIVERED";


        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrders(int page = 1, int limit = 10)
        {
            string userEmail = User.Identity.Name;
            var orders = await _orderService.GetOrderFromCustomerEmail(userEmail, page, limit);
            return Ok(orders);
        }
        [HttpGet("{orderId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            return Ok(order);
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO order)
        {
            // Try to assign the current user's email to the order
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            order.CustomerId = user.Id;
            try
            {
                var newOrder = await _orderService.CreateOrderAsync(order);
                return Ok(newOrder);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("{orderId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderDTO order)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(orderId, order);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }

        [HttpDelete("{orderId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result = await _orderService.DeleteOrderAsync(orderId);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

        //Admin section
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersAdmin(
            [FromQuery] string status = null,
            [FromQuery] string userId = null,
            [FromQuery] string paymentMethod = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var orders = await _orderService.GetOrdersFilter(status, paymentMethod, userId, page, limit);
            return Ok(orders);
        }

        //Delete array
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrders([FromBody] int[] orderIds)
        {
            foreach (var orderId in orderIds)
            {
                await _orderService.DeleteOrderAsync(orderId);
            }
            return Ok();
        }
        // Process order. Change from "PENDING" to "DELIVERING"
        [HttpPut("{orderId}/process")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = ORDER_STATUS_COMPLETED;
            await _context.SaveChangesAsync();
            return Ok(order);
        }
        // Deliver order. Change from "DELIVERING" to "DELIVERED"
        [HttpPut("{orderId}/process-delivered")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = ORDER_STATUS_DELIVERED;
            await _context.SaveChangesAsync();
            return Ok(order);
        }
        // Cancel order. Change from "PENDING" to "CANCELLED"
        [HttpPut("{orderId}/cancel")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = ORDER_STATUS_CANCELLED;
            await _context.SaveChangesAsync();
            return Ok(order);
        }





    }
}