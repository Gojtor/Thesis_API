using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Thesis_ASP.Controllers.ViewModellsForControllers;
using Thesis_ASP.Data;

namespace Thesis_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                    registeredTimne = DateTime.UtcNow,
                    DeckJson = string.Empty
                };
                var result = await userManager.CreateAsync(user, model.Password);

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
                var user = await userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }


                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    user.lastLoggedIn = DateTime.Now;

                    var insideResult = await userManager.UpdateAsync(user);

                    if (insideResult.Succeeded)
                    {
                        return Ok();
                    }
                }

                return Unauthorized("Invalid username or password.");
            }

            return BadRequest("Invalid data.");
        }

        [HttpGet("GetRegisteredUsers")]
        public async Task<IActionResult> GetRegisteredUsers()
        {
            List<User> users = await userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("AddToDeckJson")]
        public async Task<IActionResult> AddToDeckJson(string userName, string newDeckItem)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            List<string> deckList = new List<string>();

            if (!string.IsNullOrEmpty(user.DeckJson))
            {
                deckList = JsonConvert.DeserializeObject<List<string>>(user.DeckJson);
            }

            deckList.Add(newDeckItem);

            user.DeckJson = JsonConvert.SerializeObject(deckList);

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("DeckJson updated successfully.");
            }

            return BadRequest("Failed to update DeckJson.");
        }

        [HttpGet("GetDeckJson")]
        public async Task<IActionResult> GetDeckJson(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user.DeckJson);

        }

        [HttpPost("ResetDeckJson")]
        public async Task<IActionResult> ResetDeckJson(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.DeckJson = string.Empty;
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("DeckJson updated successfully.");
            }

            return BadRequest("Failed to update DeckJson.");

        }
    }
}
