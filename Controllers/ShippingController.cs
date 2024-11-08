using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Shipping;
using ecommerce_api.Services.ShippingService;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for handling shipping. Current implementation is for calculating shipping cost only and is placeholder
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        /// <summary>
        /// Calculate shipping cost for a given address. Current implementation only care about shipping method.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        ///     200: The shipping cost
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CalculateShippingCost([FromBody] ShippingRequest request)
        {
            var province = request.Province;
            var district = request.District;
            var address = request.Address;
            var method = request.Method;

            try
            {
                var shippingCost = await _shippingService.CalculateShippingCost(province, district, address, method);
                return Ok(new { shippingCost });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


    }
}