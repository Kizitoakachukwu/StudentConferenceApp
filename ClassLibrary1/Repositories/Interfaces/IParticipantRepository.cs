using DataAccessLayer.Entities;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IParticipantRepository : IGenericRepository<Participant>
    {
        Task<IEnumerable<Participant>> GetAllParticipantsWithDetailsAsync();
        Task<Participant?> GetParticipantByEmailAsync(string email);
        Task<IEnumerable<Participant>> GetParticipantsByStatusAsync(string status);
        Task<Participant?> GetParticipantWithDetailsAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
