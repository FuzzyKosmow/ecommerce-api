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
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {

                Console.WriteLine("Invalid Password");
                return BadRequest(new { message = "Invalid credentials" });
            }
            return Ok(await _jwtService.GenerateToken(user));

        }
        // Authenticate
        [HttpGet("authenticate")]
        [Authorize]
        public async Task<IActionResult> Authenticate()
        {
            // Name is configured to be the email in JWT (which should also be unique)
            return Ok(await _jwtService.GenerateToken(await _userManager.FindByEmailAsync(User.Identity.Name)));
        }
        [HttpGet("me")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            //Todo: Implement get total orders and related purchase information
            return Ok(_mapper.Map<UserDto>(user));
        }

    }
}