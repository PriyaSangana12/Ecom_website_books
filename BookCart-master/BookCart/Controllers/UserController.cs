using BookCart.Interfaces;
using BookCart.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookCart.Controllers
{
    [Route("api/[controller]")]
    public class UserController(IUserService userService, ICartService cartService) : Controller
    {
        readonly IUserService _userService = userService;
        readonly ICartService _cartService = cartService;

        /// <summary>
        /// Get the count of item in the shopping cart
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>The count of items in shopping cart</returns>
        [HttpGet("{userId}")]
        public int Get(int userId)
        {
            int cartItemCount = _cartService.GetCartItemCount(userId);
            return cartItemCount;
        }

        /// <summary>
        /// Check the availability of the username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validateUserName/{userName}")]
        public bool ValidateUserName(string userName)
        {
            return _userService.CheckUserNameAvailabity(userName);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registrationData"></param>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserRegistration registrationData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserMaster user = new()
            {
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName,
                Username = registrationData.Username,
                Password = registrationData.Password,
                Gender = registrationData.Gender,
                UserTypeId = 2
            };

            try
            {
                Console.WriteLine($"Starting registration for: {user.Username}");
                bool registrationResult = await _userService.RegisterUser(user);
                
                if (!registrationResult)
                {
                    Console.WriteLine($"Registration failed - username exists: {user.Username}");
                    return Conflict(new { message = "Username already exists" });
                }
                
                Console.WriteLine($"Registration successful for: {user.Username}");
                return Ok(new { message = "Registration successful" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }
    }
}
