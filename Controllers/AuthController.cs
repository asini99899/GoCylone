using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GoCylone.Controllers;

public class AuthController : Controller
{
    private readonly GoCyloneDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(GoCyloneDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /Login or /Auth/Login
    [HttpGet("/Login")]
    public IActionResult Login(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            ViewBag.Message = message;
        }

        // Check if user is already logged in
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Auth/Login or /Login
    [HttpPost("/Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                ViewBag.Error = "Username and password are required";
                return View();
            }

            // Check for admin credentials (hardcoded)
            if (request.UserName == "admin" && request.Password == "123456")
            {
                // Admin login
                HttpContext.Session.SetString("UserId", "0");
                HttpContext.Session.SetString("UserName", "admin");
                HttpContext.Session.SetString("Role", "admin");
                HttpContext.Session.SetString("FullName", "Administrator");

                _logger.LogInformation("Admin logged in successfully");
                return RedirectToAction("Dashboard", "Admin");
            }

            // Check for user in database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
            {
                ViewBag.Error = "Username or password is incorrect";
                return View();
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                ViewBag.Error = "Username or password is incorrect";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "User account is inactive";
                return View();
            }

            // Update last login
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            // Set session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);

            _logger.LogInformation($"User {user.UserName} logged in successfully");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ViewBag.Error = "An error occurred during login";
            return View();
        }
    }

    // GET: Auth/Register or /Register
    [HttpGet("/Register")]
    [HttpGet("/Auth/Register")]
    public IActionResult Register()
    {
        // If user is already logged in, redirect to home
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Auth/Register or /Register
    [HttpPost("/Register")]
    [HttpPost("/Auth/Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.IdNumber) ||
                string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                ModelState.AddModelError("", "All fields are required");
                return View(request);
            }

            // Validate password match
            if (request.Password != request.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(request);
            }

            // Validate password length
            if (request.Password.Length < 4)
            {
                ModelState.AddModelError("", "Password must be at least 4 characters");
                return View(request);
            }

            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(request);
            }

            // Check if ID number already exists
            var existingId = await _context.Users
                .FirstOrDefaultAsync(u => u.IdNumber == request.IdNumber);

            if (existingId != null)
            {
                ModelState.AddModelError("", "ID number already registered");
                return View(request);
            }

            // Create new user
            var user = new User
            {
                UserName = request.UserName.Trim(),
                Email = request.Email?.Trim() ?? string.Empty,
                PasswordHash = HashPassword(request.Password),
                Role = "user",
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                IdNumber = request.IdNumber.Trim(),
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"New user registered: {user.UserName}");

            // Redirect to language selection page
            return Redirect("/Auth/LanguageSelection");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            ModelState.AddModelError("", "An error occurred during registration");
            return View(request);
        }
    }

    // GET: Auth/LanguageSelection or /LanguageSelection
    [HttpGet("/Auth/LanguageSelection")]
    [HttpGet("/LanguageSelection")]
    public IActionResult LanguageSelection()
    {
        // If user is already logged in, redirect to home
        if (HttpContext.Session.GetString("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Save language preference
    [HttpPost("/Auth/SaveLanguage")]
    [HttpPost("/SaveLanguage")]
    public IActionResult SaveLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "si" && language != "ta"))
        {
            return BadRequest(new { success = false, message = "Invalid language" });
        }

        // Store language preference in session
        HttpContext.Session.SetString("PreferredLanguage", language);

        _logger.LogInformation($"Language preference set to: {language}");

        return Ok(new { success = true, message = "Language saved successfully" });
    }

    // GET: Auth/Logout or /Logout
    [HttpGet("/Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        // Clear authentication cookie
        Response.Cookies.Delete(".AspNetCore.Session");
        return Redirect("/Login");
    }

    // GET: Clear all cookies and session
    [HttpGet("/ClearCookies")]
    public IActionResult ClearCookies()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete(".AspNetCore.Session");

        // Clear all cookies
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        _logger.LogInformation("All cookies cleared");
        return Redirect("/Login");
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
        return hashOfInput == hash;
    }
}

// Request Models
public class LoginRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
