using GoCylone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers
{
    public class AdminController : Controller
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(GoCyloneDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Check if user is admin
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "admin";
        }

        // Redirect to login if not admin
        private IActionResult RedirectIfNotAdmin()
        {
            if (!IsAdmin())
            {
                _logger.LogWarning("Unauthorized admin access attempt");
                return RedirectToAction("Index", "Home");
            }
            return null!;
        }

        public IActionResult Dashboard()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            try
            {
                var totalUsers = _context.Users.Count();
                var totalBuses = _context.Buses.Count();
                var totalRoutes = _context.Routes.Count();
                var totalSchedules = _context.Schedules.Count();
                var totalBookings = _context.Bookings.Count();
                var totalRevenue = _context.Bookings
                    .Where(b => b.Status == "Confirmed")
                    .Sum(b => b.TotalFare);

                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalBuses = totalBuses;
                ViewBag.TotalRoutes = totalRoutes;
                ViewBag.TotalSchedules = totalSchedules;
                ViewBag.TotalBookings = totalBookings;
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.AdminName = HttpContext.Session.GetString("FullName");

                _logger.LogInformation("Admin accessed Dashboard");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                ViewBag.Error = "Error loading dashboard";
                return View();
            }
        }

        public IActionResult Buses()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            return View();
        }

        public IActionResult Routes()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            return View();
        }

        public IActionResult Schedules()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            return View();
        }

        public IActionResult Fares()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            return View();
        }

        public IActionResult Users()
        {
            var notAdminRedirect = RedirectIfNotAdmin();
            if (notAdminRedirect != null) return notAdminRedirect;

            return View();
        }
    }
}
