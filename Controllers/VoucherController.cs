using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Voucher;
using ecommerce_api.Services.VoucherService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for managing vouchers. Mainly used by admin role.
    /// Users can also check if a voucher is usable for their order
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        // Allow for create, delete ,disable, enable, get all and check if voucher is usable

        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        /// <summary>
        ///     Create a voucher code. Only admin can access this endpoint
        /// </summary>
        /// <param name="voucher">
        ///     Name: Name of the voucher
        ///     DiscountPercentage: Discount percentage of the voucher
        ///     Description: Description of the voucher
        ///     ExpiryDate: Expiry date of the voucher
        /// </param>
        /// <returns>
        ///     200: The created voucher (6 characters long)
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        ///     400: Bad request if the voucher is invalid
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO voucher)
        {
            var result = await _voucherService.CreateVoucherCode(voucher);
            return Ok(result);
        }
        /// <summary>
        ///     Delete a voucher code. Only admin can access this endpoint
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns>
        ///     200: The deleted voucher
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        /// </returns>
        [HttpDelete("{voucherCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVoucher(string voucherCode)
        {
            var result = await _voucherService.DeleteVoucherCode(voucherCode);
            return Ok(result);
        }
        /// <summary>
        /// Deactivate a voucher code. Only admin can access this endpoint
        ///     Deactivated voucher cannot be used
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns>
        ///     200: The deactivated voucher
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        /// </returns>
        [HttpPut("{voucherCode}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateVoucher(string voucherCode)
        {
            var result = await _voucherService.DeactivateVoucherCode(voucherCode);
            return Ok(result);
        }
        /// <summary>
        ///     Activate a voucher code. Only admin can access this endpoint
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns>
        ///     200: The activated voucher
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        /// </returns>
        [HttpPut("{voucherCode}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateVoucher(string voucherCode)
        {
            var result = await _voucherService.ActivateVoucherCode(voucherCode);
            return Ok(result);
        }
        /// <summary>
        ///     Get all voucher codes. Only admin can access this endpoint
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns>
        ///     200: The list of voucher codes
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        ///         
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVouchers(int page = 1, int limit = 10)
        {
            var result = await _voucherService.GetVoucherCodes(limit, page);
            return Ok(result);
        }
        /// <summary>
        ///    Check if a voucher code is usable. User can access this endpoint
        /// </summary>
        /// <param name="voucherCode"></param>
        /// <returns>
        ///     200: The voucher is usable or not (true or false)
        /// </returns>
        [HttpGet("{voucherCode}/can-use")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> CanUseVoucher(string voucherCode)
        {
            var result = await _voucherService.IsVoucherUsable(voucherCode);
            return Ok(result);
        }



    }
}