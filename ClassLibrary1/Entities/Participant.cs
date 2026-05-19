using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string PaperTitle { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string ParticipationMethod { get; set; } = string.Empty; // In-person, Part-time, Remote
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public Manager? Manager { get; set; }
        public ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();
        public ICollection<GeneratedDocument> GeneratedDocuments { get; set; } = new List<GeneratedDocument>();
        public object ApplicationUser { get; internal set; }
        public object Section { get; internal set; }
    }
}
