using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Entities
{
    public class GeneratedDocument
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public string DocumentType { get; set; } = string.Empty; // "Invitation", "Certificate", "ParticipantList"
        public string FilePath { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public Participant Participant { get; set; } = null!;
    }
}
