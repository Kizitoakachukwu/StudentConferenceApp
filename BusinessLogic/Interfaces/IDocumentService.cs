using StudentConferenceApp.BLL.DTOs;

namespace StudentConferenceApp.BLL.Interfaces;

public interface IDocumentService
{
    Task<string> GenerateInvitationLetterAsync(ParticipantDto participant);
    Task<string> GenerateCertificateAsync(ParticipantDto participant);
    Task<string> GenerateParticipantListAsync(List<ParticipantDto> participants);
    Task<bool> SendConfirmationEmailAsync(ParticipantDto participant);
    Task<string> GenerateRegistrationDataReportAsync(int participantId);
    Task<string> GenerateFinalReportAsync();
    Task<string> GenerateFinalReportPdfAsync();
    Task<byte[]> GenerateParticipantRosterWordAsync(IReadOnlyList<ParticipantDto> participants);
    Task<byte[]> GenerateParticipantRosterPdfAsync(IReadOnlyList<ParticipantDto> participants);
}
