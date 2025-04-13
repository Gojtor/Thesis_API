using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
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
        private readonly TCGDbContext dbContext;

        public AccountController(TCGDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }

        // Register endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                List<string> newDeck = new List<string>();
                newDeck.Add("ST01-DefaultDeck,1xST01-001,4xST01-002,4xST01-003,4xST01-004,4xST01-005,4xST01-006,4xST01-007,4xST01-008,4xST01-009,4xST01-010,2xST01-011,2xST01-012,2xST01-013,2xST01-014,2xST01-015,2xST01-016,2xST01-017");
                var user = new User
                {
                    UserName = model.UserName,
                    PasswordHash = model.Password,
                    lastLoggedIn = DateTime.UtcNow,
                    registeredTimne = DateTime.UtcNow,
                    DeckJson = JsonConvert.SerializeObject(newDeck)
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

        public class DeckAddRequest
        {
            public string UserName { get; set; }
            public string NewDeckItem { get; set; }
        }


        [HttpPost("AddToDeckJson")]
        public async Task<IActionResult> AddToDeckJson([FromBody] DeckAddRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            List<string> deckList = new List<string>();

            if (!string.IsNullOrEmpty(user.DeckJson))
            {
                deckList = JsonConvert.DeserializeObject<List<string>>(user.DeckJson);
            }

            deckList.Add(request.NewDeckItem);

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
        public async Task<IActionResult> ResetDeckJson(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

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

        public class DeckRemoveRequest
        {
            public string UserName { get; set; }
            public string DeckItemName { get; set; }
        }

        [HttpPost("RemoveDeck")]
        public async Task<IActionResult> RemoveDeck([FromBody] DeckRemoveRequest request)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return NotFound("User not found.");
            List<string> decks;
            try
            {
                decks = JsonConvert.DeserializeObject<List<string>>(user.DeckJson);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid deck JSON format: " + ex.Message);
            }
            int indexToRemove = decks.FindIndex(deck => deck.StartsWith(request.DeckItemName + ","));
            if (indexToRemove == -1)
                return NotFound($"Deck '{request.DeckItemName}' not found.");

            decks.RemoveAt(indexToRemove);
            user.DeckJson = JsonConvert.SerializeObject(decks);
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Deck removed successfully.");

            return BadRequest("Failed to update DeckJson.");
        }

        public class FriendRequest
        {
            public string SenderName { get; set; }
            public string ToUserName { get; set; }
        }

        [HttpPost("Friends/AddFriend")]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequest request)
        {
            var senderUser = await userManager.FindByNameAsync(request.SenderName);
            var toUser = await userManager.FindByNameAsync(request.ToUserName);

            if (senderUser == null || toUser == null)
            {
                return NotFound("Users not found.");
            }

            var existing = await dbContext.Friendships.FirstOrDefaultAsync(f => (f.UserName == request.SenderName && f.FriendName == request.ToUserName)
                                                                                || (f.UserName == request.ToUserName && f.FriendName == request.SenderName));
            if (existing != null)
                return BadRequest("Already friends or request pending.");

            dbContext.Friendships.Add(new Friendship
            {
                UserName = request.SenderName,
                FriendName = request.ToUserName,
                IsAccepted = false
            });
            await dbContext.SaveChangesAsync();

            return Ok("Friend request sent");

        }

        [HttpGet("Friends/GetFriends")]
        public async Task<IActionResult> GetFriendsByUsername([FromQuery] string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("User not found.");

            var friendships = await dbContext.Friendships.Where(f => (f.UserName == user.UserName || f.FriendName == user.UserName) && f.IsAccepted).ToListAsync();

            var friendNames = friendships.Select(f => f.UserName == user.UserName ? f.FriendName : f.UserName).ToList();

            var friendUsers = await userManager.Users.Where(u => friendNames.Contains(u.UserName)).Select(u => u.UserName).ToListAsync();

            return Ok(friendUsers);
        }

        [HttpGet("Friends/GetFriendRequest")]
        public async Task<IActionResult> GetFriendRequest([FromQuery] string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var pendingRequests = await dbContext.Friendships.Where(f => f.FriendName == user.UserName && !f.IsAccepted).ToListAsync();

            var senderUsernames = pendingRequests.Select(f => f.UserName).ToList();

            var senderUsers = await userManager.Users.Where(u => senderUsernames.Contains(u.UserName)).Select(u => u.UserName).ToListAsync();

            return Ok(senderUsers);
        }

        public class FriendAccept
        {
            public string SenderName { get; set; }
            public string ToUserName { get; set; }
        }

        [HttpPost("Friends/AcceptFriendRequest")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] FriendAccept req)
        {
            var fromUser = await userManager.FindByNameAsync(req.SenderName);
            var toUser = await userManager.FindByNameAsync(req.ToUserName);

            if (fromUser == null || toUser == null)
            {
                return NotFound("Users not found.");
            }

            var request = await dbContext.Friendships.FirstOrDefaultAsync(f => f.UserName == fromUser.UserName && f.FriendName == toUser.UserName && !f.IsAccepted);

            if (request == null)
            {
                return NotFound("No friend request");
            }

            request.IsAccepted = true;
            await dbContext.SaveChangesAsync();

            return Ok("Friend request accepted");
        }

        [HttpPost("Friends/DeclineFriendRequest")]
        public async Task<IActionResult> DeclineFriendRequest([FromBody] FriendAccept req)
        {
            var fromUser = await userManager.FindByNameAsync(req.SenderName);
            var toUser = await userManager.FindByNameAsync(req.ToUserName);

            if (fromUser == null || toUser == null)
            {
                return NotFound("Users not found.");
            }

            var request = await dbContext.Friendships.FirstOrDefaultAsync(f => f.UserName == fromUser.UserName && f.FriendName == toUser.UserName && !f.IsAccepted);

            if (request == null)
            {
                return NotFound("No friend request");
            }

            dbContext.Friendships.Remove(request);
            await dbContext.SaveChangesAsync();

            return Ok("Friend request declined");
        }

    }
}
