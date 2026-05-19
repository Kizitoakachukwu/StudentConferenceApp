using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentConferenceApp.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Institution { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsApproved { get; set; } = false;
        public string UserRole { get; set; } = "Participant"; // Participant or Admin

        public int? ParticipantId { get; set; }
        public Participant? Participant { get; set; }
    }
}