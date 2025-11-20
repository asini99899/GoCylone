using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GoCylone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(GoCyloneDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new { u.UserId, u.Email, u.FullName, u.Role, u.CreatedAt })
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
                return Ok(new { success = true, data = users, message = "Users retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return BadRequest(new { success = false, message = "Error retrieving users", error = ex.Message });
            }
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == id)
                    .Select(u => new { u.UserId, u.Email, u.FullName, u.Role, u.CreatedAt })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });

                return Ok(new { success = true, data = user, message = "User retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return BadRequest(new { success = false, message = "Error retrieving user", error = ex.Message });
            }
        }

        // POST: api/user/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] UserRegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { success = false, message = "Email and password are required" });

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                    return BadRequest(new { success = false, message = "User with this email already exists" });

                var user = new User
                {
                    Email = request.Email,
                    FullName = request.FullName ?? string.Empty,
                    Role = request.Role ?? "user", // Default role is "user"
                    PasswordHash = HashPassword(request.Password),
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId },
                    new { success = true, data = new { user.UserId, user.Email, user.FullName, user.Role }, message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return BadRequest(new { success = false, message = "Error registering user", error = ex.Message });
            }
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { success = false, message = "Email and password are required" });

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                    return Unauthorized(new { success = false, message = "Invalid email or password" });

                return Ok(new { success = true, data = new { user.UserId, user.Email, user.FullName, user.Role }, message = "Login successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user");
                return BadRequest(new { success = false, message = "Error logging in", error = ex.Message });
            }
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound(new { success = false, message = "User not found" });

                existingUser.FullName = request.FullName ?? existingUser.FullName;
                existingUser.Role = request.Role ?? existingUser.Role;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = new { existingUser.UserId, existingUser.Email, existingUser.FullName, existingUser.Role }, message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return BadRequest(new { success = false, message = "Error updating user", error = ex.Message });
            }
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return BadRequest(new { success = false, message = "Error deleting user", error = ex.Message });
            }
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }
    }

    public class UserRegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }

    public class UserLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserUpdateRequest
    {
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }
}
