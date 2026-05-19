using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Entities
{
    public class Manager
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PlaceOfEmployment { get; set; } = string.Empty;
        public string AcademicDegree { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        /// <summary>Optional link to an Identity user with the <c>Manager</c> role (supervisor portal).</summary>
        public string? LinkedUserId { get; set; }

        public Participant Participant { get; set; } = null!;
    }
}
