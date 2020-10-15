using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookStoreApi.Contracts;
using BookStoreApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;


        public UsersController(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            ILoggerService logger,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var description = GetControllerDescription();
            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;
                _logger.LogInfo($"{description}: Registration attempt from user {emailAddress}");
                var user = new IdentityUser { Email = emailAddress, UserName = emailAddress };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        _logger.LogError($"{description}: {error.Code} {error.Description}");
                    }

                    return InternalError($"{description}: Registration failed user {emailAddress}");
                }
                _logger.LogInfo($"{description}: {emailAddress} successfully registered");
                return Ok(new { user = user });
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// User Login Endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var description = GetControllerDescription();
            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;
                _logger.LogInfo($"{description}: Login attempt from user {emailAddress}");
                var result = await _signInManager.PasswordSignInAsync(emailAddress, password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInfo($"{description}: {emailAddress} successfully authenticated");
                    var user = await _userManager.FindByNameAsync(emailAddress);
                    var token = await GenerateJWT(user);
                    return Ok(new { token = token });
                }

                _logger.LogInfo($"{description}: {emailAddress} not authenticated");
                return Unauthorized(userDTO);
            }
            catch (Exception e)
            {
                return InternalError(e);
            }
        }

        private async Task<string> GenerateJWT(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:issuer"],
                audience: _config["Jwt:issuer"],
                claims: claims,
                notBefore: null,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerDescription()
        {
            return $"{ControllerContext.ActionDescriptor.ControllerName} - " +
                $"{ControllerContext.ActionDescriptor.ActionName}";
        }

        private ObjectResult InternalError(string msg)
        {
            _logger.LogServerError(msg);
            return StatusCode(500, "Server Error");
        }
        private ObjectResult InternalError(Exception e)
        {
            _logger.LogServerError(e);
            return StatusCode(500, "Server Error");
        }

    }
}
