namespace StudentConferenceApp.BLL.DTOs
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string PaperTitle { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string ParticipationMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

        public ManagerDto? Manager { get; set; }
    }

    public class ManagerDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PlaceOfEmployment { get; set; } = string.Empty;
        public string AcademicDegree { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
