namespace StudentConferenceApp.BLL.DTOs;

public class ParticipantDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string PaperTitle { get; set; } = string.Empty;

    public string SectionName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string ParticipationMethod { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public string? DocumentPath { get; set; }
    public string? PresentationPath { get; set; }
    public string? CertificatePath { get; set; }
    public string? InvitationPath { get; set; }
    public string? RegistrationReportZipPath { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string? ManagerFullName { get; set; }
    public string? ManagerEmployment { get; set; }
    public string? ManagerDegree { get; set; }
    public string? ManagerPosition { get; set; }
}
