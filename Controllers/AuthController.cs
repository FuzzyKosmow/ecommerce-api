using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.User;
using ecommerce_api.Models;
using ecommerce_api.Services.Cookies;
using ecommerce_api.Services.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_api.Controllers
{
    /// <summary>
    /// Controller for handling user authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;



        public AuthController(UserManager<ApplicationUser> userManager, IMapper mapper, JwtService jwtService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Register a new user. Phone number , address , province, ... are optional. They are used as a quick shortcut for user to use in checkout
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>  
        ///    200: Authentication token
        ///    400: Bad request if email is already taken
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var newUser = _mapper.Map<ApplicationUser>(dto);
            newUser.UserName = dto.Email;
            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //Add role
            await _userManager.AddToRoleAsync(newUser, "User");
            return Ok(await _jwtService.GenerateToken(newUser));
        }
        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>
        ///     200: Authentication token
        ///     400: Bad request if email is not found or password is incorrect
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                Console.WriteLine("User not found");
                Console.WriteLine(dto.Email);
                Console.WriteLine("Invalid Email");
                return BadRequest(new { message = "Invalid credentials" });
            }

            if (!user.IsActive)
            {
                Console.WriteLine("User account is disabled");
                return BadRequest(new { message = "Account is disabled" });
            }
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {

                Console.WriteLine("Invalid Password");
                return BadRequest(new { message = "Invalid credentials" });
            }
            return Ok(await _jwtService.GenerateToken(user));

        }
        /// <summary>
        /// Authenticate with JWT. This is used to refresh and get a new token. The old token is still valid until it expires
        /// Only authenticated user can access this endpoint. It is quite scuffed as of now. Best to just re-login if token is invalid
        /// </summary>
        /// <returns>
        ///     200: Authentication token
        ///     401: Unauthorized if token is invalid
        ///     403: Forbidden if user is not authenticated
        /// </returns>
        [HttpGet("authenticate")]
        [Authorize]
        public async Task<IActionResult> Authenticate()
        {
            // Name is configured to be the email in JWT (which should also be unique)
            return Ok(await _jwtService.GenerateToken(await _userManager.FindByEmailAsync(User.Identity.Name)));
        }
        /// <summary>
        /// Get user information. Only authenticated user can access this endpoint
        /// </summary>
        /// <returns>
        ///     200: User information (UserDto)
        ///     401: Unauthorized if token is invalid
        ///     403: Forbidden if user is not authenticated
        /// </returns>
        [HttpGet("me")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var role = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = role.FirstOrDefault();
            //Todo: Implement get total orders and related purchase information
            return Ok(userDto);
        }

    }
}