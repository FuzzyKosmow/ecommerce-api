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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO voucher)
        {
            var result = await _voucherService.CreateVoucherCode(voucher);
            return Ok(result);
        }

        [HttpDelete("{voucherCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVoucher(string voucherCode)
        {
            var result = await _voucherService.DeleteVoucherCode(voucherCode);
            return Ok(result);
        }

        [HttpPut("{voucherCode}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateVoucher(string voucherCode)
        {
            var result = await _voucherService.DeactivateVoucherCode(voucherCode);
            return Ok(result);
        }
        [HttpPut("{voucherCode}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateVoucher(string voucherCode)
        {
            var result = await _voucherService.ActivateVoucherCode(voucherCode);
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVouchers(int page = 1, int limit = 10)
        {
            var result = await _voucherService.GetVoucherCodes(limit, page);
            return Ok(result);
        }

        [HttpGet("{voucherCode}/can-use")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> CanUseVoucher(string voucherCode)
        {
            var result = await _voucherService.IsVoucherUsable(voucherCode);
            return Ok(result);
        }



    }
}