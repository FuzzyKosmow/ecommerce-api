using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecommerce_api.Services
{
    public class PromotionService
    {
        private readonly AppDbContext _context;

        public PromotionService(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        ///     Clear all discounts currently applied to products
        /// </summary>
        public async Task ClearAllDiscounts()
        {
            var products = _context.Products;
            foreach (var product in products)
            {
                product.DiscountPrice = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearDiscount(List<int> productIds)
        {
            foreach (var productId in productIds)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }
                product.DiscountPrice = null;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Clear all expired promotions and also clear discounts applied to products in those promotions
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ClearExpiredPromotions()
        {
            var promotions = _context.Promotions;
            foreach (var promotion in promotions)
            {
                if (promotion.ValidUntil < DateTime.Now)
                {
                    promotion.IsActive = false;
                }
            }
            // Clear discounts in products in promotions
            await ClearDiscount(promotions.SelectMany(p => p.ApplicableProductIds).ToList());

        }
        /// <summary>
        ///    Apply promotion to products in the promotion
        /// </summary>
        /// <param name="promotionId">
        ///     Id of the promotion to apply
        /// </param>
        /// <returns></returns>

        public async Task ApplyPromotion(int promotionId)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId);

            if (promotion == null)
            {
                throw new Exception("Promotion not found");
            }


            if (promotion.ValidUntil < DateTime.Now)
            {
                throw new Exception("Promotion is expired");
            }

            foreach (var productId in promotion.ApplicableProductIds)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }
                if (product.DiscountPrice != null || product.DiscountPrice != 0)
                {
                    Console.WriteLine("WARN: Product already has a discount applied. Overwriting");
                    Console.WriteLine("Old discount price: " + product.DiscountPrice);
                }
                product.DiscountPrice = product.Price - (product.Price * promotion.DiscountPercentage / 100);
            }
            promotion.IsActive = true;
            await _context.SaveChangesAsync();

        }
    }
}