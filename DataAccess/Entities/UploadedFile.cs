using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public string FileType { get; set; } = string.Empty; // "Paper" or "Presentation"
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Participant Participant { get; set; } = null!;
    }
}
