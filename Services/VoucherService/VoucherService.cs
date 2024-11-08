using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Voucher;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Services.VoucherService
{
    public class VoucherService : IVoucherService
    {

        private readonly AppDbContext _context;
        private int VoucherCodeLength = 6;

        public VoucherService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string?> CreateVoucherCode(CreateVoucherDTO voucher)
        {
            string newVoucherCode = await GenerateVoucherCode();
            var newVoucher = new Models.Voucher
            {
                Name = voucher.Name,
                Code = newVoucherCode,
                DiscountPercentage = voucher.DiscountPercentage,
                Description = voucher.Description,
                ExpiryDate = voucher.ExpiryDate,
                IsActive = true,
                IsUsed = false
            };
            await _context.Vouchers.AddAsync(newVoucher);
            await _context.SaveChangesAsync();
            return newVoucherCode;
        }

        public async Task<bool> DeactivateVoucherCode(string voucherCode)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode);
            if (voucher == null)
            {
                return false;
            }
            voucher.IsActive = false;
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ActivateVoucherCode(string voucherCode)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode);
            if (voucher == null)
            {
                return false;
            }
            voucher.IsActive = true;
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVoucherCode(string voucherCode)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode);
            if (voucher == null)
            {
                return false;
            }
            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<string> GenerateVoucherCode()
        {
            // Generate a random voucher code. 6 characters long and does not exist in the database.
            // All case up.
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            // Loop and check if the generated code exists in the database
            while (true)
            {
                string code = new string(Enumerable.Repeat(chars, VoucherCodeLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                if (!_context.Vouchers.Any(v => v.Code == code))
                {
                    return Task.FromResult(code);
                }
            }

        }

        public async Task<List<string>> GetVoucherCodes(int limit, int page)
        {
            return await _context.Vouchers.Skip((page - 1) * limit).Take(limit).Select(v => v.Code).ToListAsync();
        }

        public async Task<bool> IsVoucherUsable(string voucherCode)
        {
            // If UserID is null, then the voucher is applicable to all users. Meaning it is not used 
            // If the voucher is not active, then it cannot be used
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode);
            if (voucher == null || !voucher.IsActive || voucher.IsUsed)
            {
                return false;
            }
            return true;
        }

    }
}