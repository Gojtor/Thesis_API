using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thesis_ASP.Controllers.ViewModellsForControllers;
using Thesis_ASP.Data;

namespace Thesis_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Register endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    PasswordHash = model.Password,
                    lastLoggedIn = DateTime.UtcNow,
                    registeredTimne = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Registration successful!" });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Invalid data.");
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return Ok();
                }

                return Unauthorized("Invalid username or password.");
            }

            return BadRequest("Invalid data.");
        }
    }
}
