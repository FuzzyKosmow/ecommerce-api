using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Voucher;

namespace ecommerce_api.Services.VoucherService
{
    public interface IVoucherService
    {
        Task<bool> IsVoucherUsable(string voucherCode);
        Task<List<string>> GetVoucherCodes(int limit, int page);
        Task<string?> CreateVoucherCode(CreateVoucherDTO voucher);

        Task<bool> DeleteVoucherCode(string voucherCode);
        Task<bool> DeactivateVoucherCode(string voucherCode);
        Task<bool> ActivateVoucherCode(string voucherCode);

        /// <summary>
        /// Generate a random voucher code. 6 characters long and does not exist in the database
        /// </summary>
        Task<string> GenerateVoucherCode();
        
        Task<decimal> GetDiscountAmount(string voucherCode);
    }
}