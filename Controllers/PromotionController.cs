using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Promotion;
using ecommerce_api.Models;
using ecommerce_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PromotionService _promotionService;
        private readonly IMapper _mapper;
        public PromotionController(AppDbContext context, PromotionService promotionService, IMapper mapper)
        {
            _context = context;

            _promotionService = promotionService;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Promotion>>> GetPromotions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string includeProducts = "false"
        )
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Invalid page or pageSize");
            }
            var promotions = await _context.Promotions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (includeProducts == "true")
            {
                foreach (var promotion in promotions)
                {
                    promotion.Products = await _context.Products.Where(p => promotion.ApplicableProductIds.Contains(p.Id)).ToListAsync();
                }
            }
            return Ok(promotions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreatePromotion([FromBody] CreatePromotionDTO promotion)
        {
            var promotionModel = _mapper.Map<Promotion>(promotion);
            _context.Promotions.Add(promotionModel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            await _promotionService.ClearDiscount(promotion.ApplicableProductIds);
            return Ok("Promotion deleted");
        }

        [HttpPost("clear-expired")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ClearExpiredPromotions()
        {
            await _promotionService.ClearExpiredPromotions();
            return Ok("Expired promotions cleared");
        }
        /// <summary>
        ///     Apply a promotion to products in the promotion
        /// </summary>
        /// <param name="id">
        ///     Promotion id
        /// </param>
        [HttpPost("apply/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ApplyPromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            try
            {
                await _promotionService.ApplyPromotion(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok("Promotion applied");
        }
        /// <summary>
        ///    Deactivate a promotion. Also clear discounts applied to products in the promotion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivatePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            promotion.IsActive = false;
            await _context.SaveChangesAsync();
            await _promotionService.ClearDiscount(promotion.ApplicableProductIds);
            return Ok("Promotion deactivated");
        }


        // Promo code section




    }
}