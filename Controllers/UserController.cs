using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ecommerce_api.DTO.User;
using System.Security.Claims;
namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for managing users. Mainly used by admin role. 
    ///  Main features: create, update, delete, get users, get user orders, get user reviews
    ///     
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers(int page = 1, int limit = 10)
        {
            var users = _userManager.Users.Skip((page - 1) * limit).Take(limit).ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("{id}/disable")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DisableUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            return Ok(user);
        }
        [HttpPost("{id}/enable")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EnableUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return Ok(user);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateUser(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser
            {
                FullName = registerUserDto.FullName,
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                Address = registerUserDto.Address,
                PhoneNumber = registerUserDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (result.Succeeded)
            {
                return Ok("User created successfully");
            }
            return BadRequest(result.Errors);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> UpdateUser(string id, UpdateUserDTO updateUserDTO)
        {
            //If role is not admin. Then user can only update his own profile. Id must match with the logged in user id
            if (!User.IsInRole("Admin") && User.FindFirst(ClaimTypes.NameIdentifier).Value != id)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (updateUserDTO.FullName != null && updateUserDTO.FullName.Length > 0)
            {
                user.FullName = updateUserDTO.FullName;
            }
            if (updateUserDTO.Address != null && updateUserDTO.Address.Length > 0)
            {
                user.Address = updateUserDTO.Address;
            }
            if (updateUserDTO.PhoneNumber != null && updateUserDTO.PhoneNumber.Length > 0)
            {
                user.PhoneNumber = updateUserDTO.PhoneNumber;
            }

            await _userManager.UpdateAsync(user);
            return Ok(user);

        }
    }
}