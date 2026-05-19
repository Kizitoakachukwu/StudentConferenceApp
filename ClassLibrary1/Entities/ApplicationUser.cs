namespace DataAccessLayer.Entities
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    namespace StudentConferenceApp.DAL.Entities
    {
        public class ApplicationUser : IdentityUser
        {
            // Additional fields specific to your project
            [Required]
            [MaxLength(100)]
            public string FullName { get; set; } = string.Empty;

            [MaxLength(200)]
            public string? Institution { get; set; }

            public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

            // Navigation property to link with Participant data
            public int? ParticipantId { get; set; }
            public Participant? Participant { get; set; }

            // Useful flags
            public bool IsApproved { get; set; } = false;
            public DateTime? LastLoginDate { get; set; }

            // Optional: Store role as string for easy checking
            [MaxLength(20)]
            public string UserRole { get; set; } = "Participant"; // "Participant" or "Admin"
        }
    }
}
