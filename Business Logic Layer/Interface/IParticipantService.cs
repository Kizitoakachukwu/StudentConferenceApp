using StudentConferenceApp.BLL.DTOs;


namespace StudentConferenceApp.BLL.Interfaces
{
    public interface IParticipantService
    {
        Task<int> RegisterParticipantAsync(ParticipantRegisterDto dto);
        Task<List<ParticipantDto>> GetAllParticipantsAsync();
        Task<ParticipantDto?> GetParticipantByIdAsync(int id);
        Task<bool> ApproveParticipantAsync(int id);
        Task<bool> RejectParticipantAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
    }
}