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
    /// Methods for user to update their own profile are also included   
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
        /// <summary>
        /// Get a list of users. Only admin can access this endpoint
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns>
        ///     200: List of users
        ///     401: Unauthorized if the user is not admin
        ///     404: Not found if the user is not found
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers(int page = 1, int limit = 10)
        {
            var users = _userManager.Users.Skip((page - 1) * limit).Take(limit).ToList();
            return Ok(users);
        }
        /// <summary>
        ///     Get a user by id. Only admin can access this endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     200: The user
        ///     401: Unauthorized if the user is not admin
        ///     404: Not found if the user is not found
        ///     403: Forbidden if the user is not admin
        /// </returns>
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

        /// <summary>
        /// Disable a user. Only admin can access this endpoint
        /// This will make the user inactive and unable to login.
        /// However, the old login token is still valid for fetching related data until it expires
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     200: The user
        ///     401: Unauthorized if the user is not admin
        ///     404: Not found if the user is not found
        /// </returns>
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

        /// <summary>
        ///     Enable a user. Only admin can access this endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     200: The user
        ///     401: Unauthorized if the user is not admin
        ///     404: Not found if the user is not found
        ///     403: Forbidden if the user is not admin
        /// </returns>
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
        /// <summary>
        ///     Create a new user. Only admin can access this endpoint
        ///     This endpoint is different from register as it does not require email confirmation
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns>
        ///     200: User created successfully
        ///     400: Bad request if the user is not created
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin
        ///     500: Internal server error
        /// </returns>
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
        /// <summary>
        ///     Update a user. Only admin can access this endpoint
        ///     User can only update their own profile if they are not admin
        ///     Admin can update any user profile    
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUserDTO"></param>
        /// <returns>
        ///     200: The updated user
        ///     401: Unauthorized if the user is not admin
        ///     403: Forbidden if the user is not admin and id does not match with the logged in user id
        ///     404: Not found if the user is not found
        /// </returns>
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