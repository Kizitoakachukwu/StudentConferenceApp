using StudentConferenceApp.BLL.DTOs;

namespace StudentConferenceApp.BLL.Interfaces
{
    public interface IParticipantService
    {
        Task<List<ParticipantDto>> GetAllParticipantsAsync();
        Task<List<ParticipantDto>> GetAllParticipantsWithDetailsAsync();
        Task<ParticipantDto?> GetParticipantByIdAsync(int id);
        Task<bool> ApproveParticipantAsync(int id);
        Task<bool> RejectParticipantAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<ParticipantDto?> GetParticipantByEmail(string? email);
        Task<List<ParticipantDto>> GetSupervisedParticipantsAsync(string managerUserId);
        /// <summary>Sets participant status when their manager row is linked to <paramref name="managerUserId"/>.</summary>
        Task<bool> SetSupervisedParticipantStatusAsync(string managerUserId, int participantId, string status);
        Task<bool> SetParticipantStatusAsync(int participantId, string status);
        Task RegisterAccountAsync(AccountRegisterDto dto);
        Task<int> SubmitConferenceRegistrationAsync(string userId, ParticipantRegisterDto dto);
    }
}
