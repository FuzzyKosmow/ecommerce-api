using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Order;
using ecommerce_api.Models;
using ecommerce_api.Services.PaymentService;
using ecommerce_api.Services.ProductService;
using ecommerce_api.Services.ShippingService;
using ecommerce_api.Services.TaxService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Services.OrderService
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShippingService _shippingService;
        private readonly ITaxService _taxService;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(AppDbContext context, IProductService productService, UserManager<ApplicationUser> userManager, IMapper mapper, IShippingService shippingService, ITaxService taxService, IPaymentService paymentService)
        {
            _context = context;
            _productService = productService;
            _userManager = userManager;
            _mapper = mapper;
            _shippingService = shippingService;
            _taxService = taxService;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(CreateOrderDTO order)
        {
            decimal shippingFee = await _shippingService.CalculateShippingCost(order.Province, order.District, order.Address, order.ShippingMethod);
            string trackingId = await _shippingService.GetTrackingNumber();
            // Create a new order
            Order newOrder = new Order
            {
                UserId = order.CustomerId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                CustomerName = order.CustomerName,  
                // Contact props
                Address = order.Address,
                Province = order.Province,
                District = order.District,
                PhoneNumber = order.PhoneNumber,
                // Shipping props
                ShippingFee = shippingFee,
                ShippingMethod = order.ShippingMethod,
                TrackingID = trackingId,
                // Set the payment props
                PaymentMethod = order.PaymentMethod,
                CardCvv = order.CardCvv ?? "",
                CardExpireDate = order.CardExpireDate ?? "",
                CardHolder = order.CardHolder ?? "",
                CardNumber = order.CardNumber ?? "",

            };
            decimal subTotal = 0;

            newOrder.OrderDetails = new List<OrderDetail>();
            Console.WriteLine("Adding order details");
            Console.WriteLine("Order details count: " + order.OrderDetails.Count);
            // Add order details
            foreach (var orderDetail in order.OrderDetails)
            {
                Product? product = await _productService.GetProduct(orderDetail.ProductId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }
                // Check matching if color and storage exist. Check if storage and modifier match
                if (product.Colors != null && product.Colors.Count > 0 && !product.Colors.Contains(orderDetail.Color))
                {
                    throw new Exception("Color not found");
                }
                if (product.StorageOptions != null && product.StorageOptions.Count > 0 && !product.StorageOptions.Contains(orderDetail.Storage))
                {
                    throw new Exception("Storage not found");
                }
                int storageIndex = product.StorageOptions.IndexOf(orderDetail.Storage);
                if (product.StorageModifiers != null && product.StorageModifiers.Count > 0 && product.StorageModifiers[storageIndex] != orderDetail.StorageModifier)
                {
                    throw new Exception("Storage modifier not match");

                }
                newOrder.OrderDetails.Add(new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Quantity = orderDetail.Quantity,
                    Price = (decimal)(product.DiscountPrice != null && product.DiscountPrice > 0 ? product.DiscountPrice : product.Price),
                    Color = orderDetail.Color,
                    Storage = orderDetail.Storage,
                    StorageModifier = orderDetail.StorageModifier

                });
                Console.WriteLine("Adding a product with price: " + (decimal)(product.DiscountPrice != null && product.DiscountPrice > 0 ? product.DiscountPrice : product.Price) + " and quantity: " + orderDetail.Quantity);
                subTotal += (decimal)(product.DiscountPrice != null && product.DiscountPrice > 0 ? product.DiscountPrice : product.Price) * orderDetail.Quantity * orderDetail.StorageModifier;

            }



            // Calculate reduced value from promo code(voucher) if any
            if (order.PromoCodeApplied != null)
            {
                Voucher? promo = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == order.PromoCodeApplied);
                if (promo != null && promo.IsActive && !promo.IsUsed && promo.ExpiryDate > DateTime.Now)
                {
                    newOrder.PromoCodeApplied = promo.Code;
                    newOrder.PromoCodeDiscount = subTotal * promo.DiscountPercentage / 100;
                    promo.IsUsed = true;
                    promo.UserId = order.CustomerId;
                    subTotal -= (decimal)newOrder.PromoCodeDiscount;
                }
            }
            // Calculate tax
            newOrder.Tax = await _taxService.CalculateTax(subTotal);
            // Call payment service to charge the card

            bool paymentResult = await _paymentService.ChargeCreditCard(newOrder.CardNumber, newOrder.CardHolder, newOrder.CardExpireDate, newOrder.CardCvv, newOrder.Total);
            if (!paymentResult)
            {
                throw new Exception("Payment failed");
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

        public async Task<decimal> TotalValueFromCustomer(string id)
        {
            var orders = await GetOrderFromCustomer(id);
            if (orders == null)
            {
                return 0;
            }
            return orders.Sum(o => o.Total);
        }

    }
}