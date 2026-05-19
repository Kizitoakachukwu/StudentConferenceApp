using StudentConferenceApp.DAL.Entities;

namespace DataAccess.Entities
{
    public class Participant
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string University { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>Conference paper title (topic).</summary>
        public string PaperTitle { get; set; } = string.Empty;

        public int? SectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public string ParticipationMethod { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? DocumentPath { get; set; }

        public string? CertificatePath { get; set; }

        public string? InvitationPath { get; set; }

        /// <summary>ZIP containing registration table Word + Excel files.</summary>
        public string? RegistrationReportZipPath { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public Manager? Manager { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
        public string PresentationPath { get; set; } = string.Empty;
        public string Institution { get; set; } =string.Empty;
    }
}
