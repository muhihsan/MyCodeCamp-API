using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using MyCodeCamp.Filters;
using MyCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Controllers
{
    public class AuthController : Controller
    {
        private CampContext _context;
        private SignInManager<CampUser> _signInMananger;
        private ILogger<AuthController> _logger;

        public AuthController(CampContext context, SignInManager<CampUser> signInManager, ILogger<AuthController> logger)
        {
            _context = context;
            _signInMananger = signInManager;
            _logger = logger;
        }

        [HttpPost("api/auth/login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] CredentialModel model)
        {
            try
            {
                var result = await _signInMananger.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while logging in: {ex}");
            }

            return BadRequest("Failed to login");
        }
    }
}
