using DataAccessLayer.Entities;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IManagerRepository : IGenericRepository<Manager>
    {
        Task<Manager?> GetManagerByParticipantIdAsync(int participantId);
        Task<Manager?> GetManagerWithParticipantAsync(int managerId);
    }
}
