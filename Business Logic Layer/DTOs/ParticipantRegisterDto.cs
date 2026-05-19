using System.ComponentModel.DataAnnotations;

namespace StudentConferenceApp.BLL.DTOs
{
    public class ParticipantRegisterDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Institution is required")]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Paper title is required")]
        [MaxLength(300)]
        public string PaperTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section is required")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Participation method is required")]
        public string ParticipationMethod { get; set; } = string.Empty; // In-person, Part-time, Remote

        // Manager Information (Optional)
        [MaxLength(100)]
        public string? ManagerFullName { get; set; }

        [MaxLength(200)]
        public string? ManagerEmployment { get; set; }

        [MaxLength(100)]
        public string? ManagerDegree { get; set; }

        [MaxLength(100)]
        public string? ManagerPosition { get; set; }
    }
}
