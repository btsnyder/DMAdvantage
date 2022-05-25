using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DMAdvantage.Data;

namespace DMAdvantage.Server.Controllers
{
    [Route("api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        protected readonly DMContext _context;

        public AccountController(DMContext context,
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration config)
        {
            _context = context;
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
                            return Created("", CreateLogin(user));
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

        [Route("refresh")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshToken()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);
            return Ok(CreateLogin(currentUser));
        }

        private LoginResponse CreateLogin(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            var tokenVar = _config["Tokens:Key"];
            var tokenKey = Environment.GetEnvironmentVariable(tokenVar) ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new LoginResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
