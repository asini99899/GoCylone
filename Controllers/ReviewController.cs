using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers;

[Route("Review")]
public class ReviewController : Controller
{
    private readonly GoCyloneDbContext _context;
    private readonly ILogger<ReviewController> _logger;

    public ReviewController(GoCyloneDbContext context, ILogger<ReviewController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Review/Index - Display all reviews
    [HttpGet]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reviews");
            return BadRequest("Error loading reviews");
        }
    }

    // POST: Review/AddReview - Add a new review
    [HttpPost("AddReview")]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Please login to add a review" });

            if (!int.TryParse(userId, out var userIdInt))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Title) || request.Stars < 1 || request.Stars > 5)
                return BadRequest(new { success = false, message = "Please provide a valid title and stars (1-5)" });

            // Check if user already reviewed
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userIdInt);

            if (existingReview != null)
                return BadRequest(new { success = false, message = "You have already submitted a review. Edit or delete it to submit a new one." });

            var review = new Review
            {
                UserId = userIdInt,
                Stars = request.Stars,
                Title = request.Title.Trim(),
                Comment = request.Comment?.Trim() ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review added by user {userIdInt}");

            return Ok(new { success = true, message = "Review added successfully!", reviewId = review.ReviewId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding review");
            return BadRequest(new { success = false, message = "Error adding review: " + ex.Message });
        }
    }

    // PUT: Review/EditReview/:id - Edit own review
    [HttpPut("EditReview/{id}")]
    public async Task<IActionResult> EditReview(int id, [FromBody] EditReviewRequest request)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Please login to edit a review" });

            if (!int.TryParse(userId, out var userIdInt))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null)
                return NotFound(new { success = false, message = "Review not found" });

            // Check if user owns the review
            if (review.UserId != userIdInt)
                return Forbid();

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Title) || request.Stars < 1 || request.Stars > 5)
                return BadRequest(new { success = false, message = "Please provide a valid title and stars (1-5)" });

            review.Title = request.Title.Trim();
            review.Comment = request.Comment?.Trim() ?? string.Empty;
            review.Stars = request.Stars;
            review.UpdatedAt = DateTime.Now;

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review {id} edited by user {userIdInt}");

            return Ok(new { success = true, message = "Review updated successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing review");
            return BadRequest(new { success = false, message = "Error editing review: " + ex.Message });
        }
    }

    // DELETE: Review/DeleteReview/:id - Delete own review
    [HttpDelete("DeleteReview/{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Please login to delete a review" });

            if (!int.TryParse(userId, out var userIdInt))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null)
                return NotFound(new { success = false, message = "Review not found" });

            // Check if user owns the review
            if (review.UserId != userIdInt)
                return Forbid();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review {id} deleted by user {userIdInt}");

            return Ok(new { success = true, message = "Review deleted successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review");
            return BadRequest(new { success = false, message = "Error deleting review: " + ex.Message });
        }
    }

    // GET: /api/reviews - Get all reviews as JSON
    [HttpGet("/api/reviews")]
    public async Task<IActionResult> GetAllReviewsJson()
    {
        try
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    reviewId = r.ReviewId,
                    userId = r.UserId,
                    stars = r.Stars,
                    title = r.Title,
                    comment = r.Comment,
                    createdAt = r.CreatedAt,
                    user = new
                    {
                        userId = r.User!.UserId,
                        userName = r.User.UserName,
                        fullName = r.User.FullName
                    }
                })
                .ToListAsync();

            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reviews");
            return BadRequest(new { success = false, message = "Error loading reviews" });
        }
    }

    // GET: Review/GetStats - Get review statistics
    [HttpGet("GetStats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var totalReviews = await _context.Reviews.CountAsync();
            var avgRating = totalReviews > 0 ? await _context.Reviews.AverageAsync(r => r.Stars) : 0;

            var starsDistribution = new Dictionary<int, int>();
            for (int i = 1; i <= 5; i++)
            {
                starsDistribution[i] = await _context.Reviews.CountAsync(r => r.Stars == i);
            }

            return Ok(new
            {
                success = true,
                totalReviews = totalReviews,
                averageRating = Math.Round(avgRating, 2),
                starsDistribution = starsDistribution
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting review stats");
            return BadRequest(new { success = false, message = "Error getting stats: " + ex.Message });
        }
    }

    // GET: Review/UserReview - Get current user's review
    [HttpGet("UserReview")]
    public async Task<IActionResult> UserReview()
    {
        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "Please login" });

            if (!int.TryParse(userId, out var userIdInt))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userIdInt);

            if (review == null)
                return Ok(new { success = true, review = (object?)null });

            return Ok(new { success = true, review });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user review");
            return BadRequest(new { success = false, message = "Error getting review: " + ex.Message });
        }
    }
}

// Request/Response Models
public class AddReviewRequest
{
    public int Stars { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class EditReviewRequest
{
    public int Stars { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
