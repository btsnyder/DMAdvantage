using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AccountController(ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        [Route("token")]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);

                    if (user != null)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                        if (result.Succeeded)
                        {
                            // Create the Token
                            var claims = new[]
                            {
                                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                            };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));

                            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                _config["Tokens:Issuer"],
                                _config["Tokens:Audience"],
                                claims,
                                expires: DateTime.UtcNow.AddDays(7),
                                signingCredentials: credentials);

                            var userModel = new LoginResponse
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Token = new JwtSecurityTokenHandler().WriteToken(token)
                            };

                            return Created("", userModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create token: {ex}");
            }

            return BadRequest();
        }
    }
}
