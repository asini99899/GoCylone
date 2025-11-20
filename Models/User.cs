namespace GoCylone.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // Username for login
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user"; // "admin" or "user"
        public string FullName { get; set; } = string.Empty; // Full name of user
        public string PhoneNumber { get; set; } = string.Empty; // Phone number for contact
        public string IdNumber { get; set; } = string.Empty; // ID number (NIC, Passport, etc.)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
