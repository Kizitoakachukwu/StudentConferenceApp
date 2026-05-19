using System;
using DataAccessLayer.Entities;

namespace StudentConferenceApp.BLL.Interfaces
{
    public interface IDocumentService
    {
        Task<string> GenerateInvitationLetterAsync(Participant participant);
        Task<string> GenerateCertificateAsync(Participant participant);
        Task<string> GenerateParticipantListAsync(List<Participant> participants);
        Task<bool> SendConfirmationEmailAsync(Participant participant);
    }
}