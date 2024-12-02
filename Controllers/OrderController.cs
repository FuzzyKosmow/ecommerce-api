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
    /// <summary>
    ///    Controller for handling orders
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        public readonly string ORDER_STATUS_PENDING = "PENDING";
        public readonly string ORDER_STATUS_COMPLETED = "DELIVERING";
        public readonly string ORDER_STATUS_CANCELLED = "CANCELLED";
        public readonly string ORDER_STATUS_DELIVERED = "DELIVERED";

        public readonly decimal TAX_RATE = 0.1m;

        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _orderService = orderService;
            _userManager = userManager;
        }
        /// <summary>
        /// Get all orders of the current user
        /// </summary>
        /// <param name="page">
        ///     Page number
        /// </param>
        /// <param name="limit">
        ///     Number of items per page
        /// </param>
        /// <returns>
        ///     List of orders
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrders(int page = 1, int limit = 10)
        {
            string userEmail = User.Identity.Name;
            var orders = await _orderService.GetOrderFromCustomerEmail(userEmail, page, limit);
            return Ok(orders);
        }
        /// <summary>
        /// Get a specific order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        ///     200: Full order information, including details and other user information stored in the order.
        ///     404: Not found if the order is not found
        /// </returns>
        [HttpGet("{orderId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            return Ok(order);
        }

        /// <summary>
        ///     Create a new order. Requires authentication to get the current user's email.
        ///     The order will be created with status "PENDING". Payment as of now is not required.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>
        ///     200: Order information on success
        ///     400: Bad request if the order is invalid. Including missing fields, invalid product or quantity, etc.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO order)
        {
            // Try to assign the current user's email to the order
            if (User.Identity.Name == null)
            {
                Console.WriteLine("WARN: User not found. Order will be created without user ID");  
                order.CustomerId = null;
            }
            else {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    Console.WriteLine("WARN: User not found. Order will be created without user ID");
                    order.CustomerId = null;
                }
                else
                {
                    order.CustomerId = user.Id;   
                }
            }
           
            
            try
            {
                var newOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder), new { orderId = newOrder.Id }, newOrder);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        /// <summary>
        ///     Update an order. Only the current user can update their own order.
        /// </summary>
        /// <param name="orderId">
        ///     Order ID to update    
        /// </param>
        /// <param name="order">
        ///     Order information to update. Only the fields that need to be updated are required. All other fields can be null.
        ///  
        /// </param>
        /// <returns>
        ///     200: Updated order information
        ///     404: Not found if the order is not found
        /// </returns>
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
        /// <summary>
        ///    Delete an order. Only the current user can delete their own order.
        ///    
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        ///     200: Success
        ///     404: Not found if the order is not found
        ///     403: Forbidden if the order does not belong to the user
        ///     401: Unauthorized if the user is not authenticated
        /// </returns>
        [HttpDelete("{orderId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            // If admin is deleting, no need to check the user, just delete
            if (User.IsInRole("Admin"))
            {
                await _orderService.DeleteOrderAsync(orderId);
                return Ok();
            }
            // If user is deleting, check if the order belongs to the user
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (order.UserId != user.Id)
            {
                return Forbid();
            }
            await _orderService.DeleteOrderAsync(orderId);
            return Ok();


        }

        //Admin section

        /// <summary>
        /// Get all orders with filters. Only admin can access this endpoint
        /// </summary>
        /// <param name="status">
        ///     Filter by status. Include "PENDING", "DELIVERING", "DELIVERED", "CANCELLED"
        /// </param>
        /// <param name="userId"></param>
        /// <param name="paymentMethod">
        ///     Filter by payment method. Include "Cash", "Credit Card", "Visa"
        /// </param>
        /// <param name="page">
        ///     Page number
        /// </param>
        /// <param name="limit">
        ///      Number of items per page
        /// </param>
        /// <returns>
        ///     List of orders based on filter
        /// </returns>
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

        /// <summary>
        /// Delete multiple orders. Only admin can access this endpoint
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns>
        ///     200: Success
        ///     401: Unauthorized if the user is not authenticated
        ///    403: Forbidden if the user is not an admin
        /// </returns>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrders([FromBody] int[] orderIds)
        {
            foreach (var orderId in orderIds)
            {
                var res = await _orderService.DeleteOrderAsync(orderId);
                if (!res)
                {
                    // WARN but continue
                    Console.WriteLine($"WARN: Order {orderId} not found. Skipping delete");
                    continue;
                }
            }
            return Ok();
        }
        /// <summary>
        /// Process order. Change from "PENDING" to "DELIVERING". Only admin can access this endpoint
        /// Used when the order is ready to be delivered, process by admin
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        ///     200: Order information
        ///     404: Not found if the order is not found
        ///     401: Unauthorized if the user is not authenticated
        ///     403: Forbidden if the user is not an admin
        /// </returns>
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
        /// <summary>
        ///     Process delivered order. Change from "DELIVERING" to "DELIVERED". Only admin can access this endpoint
        ///     Used when the order is delivered to the customer
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        ///     200: Order information
        ///     404: Not found if the order is not found
        ///     401: Unauthorized if the user is not authenticated
        ///     403: Forbidden if the user is not an admin
        /// </returns>
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
        /// <summary>
        ///     Cancel order. Change from "PENDING" to "CANCELLED". Only admin can access this endpoint
        ///     Used when the order is cancelled by the customer
        ///     If the user is cancelling, check if the order belongs to the user
        ///     If the admin is cancelling, no need to check the user
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>
        ///     200: Order information
        ///     404: Not found if the order is not found
        ///     401: Unauthorized if the user is not authenticated
        ///     403: Forbidden if the user is not an admin or the order does not belong to the user
        /// </returns>
        [HttpPut("{orderId}/cancel")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            // If user is cancelling, check if the order belongs to the user
            if (User.IsInRole("User"))
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (order.UserId != user.Id)
                {
                    return Forbid();
                }
            }

            order.Status = ORDER_STATUS_CANCELLED;
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        /// <summary>
        ///   Get tax rate
        ///   Used to calculate tax for the order. 
        ///   
        /// </summary>
        /// <returns></returns>
        [HttpGet("tax-rate")]
        public IActionResult GetTaxRate()
        {
            return Ok(new { taxRate = TAX_RATE });
        }





    }
}