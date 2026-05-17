using System;

namespace ClassLibrary1.Models
{
    public class Profile
    {
        public Guid Id { get; set; }             // Matches Supabase Auth user ID
        public string Username { get; set; }     // First Name or display name
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }  // Optional
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ProfilePictureUrl { get; set; } // Optional
        public string Bio { get; set; }          // Optional mental wellness info
    }
}