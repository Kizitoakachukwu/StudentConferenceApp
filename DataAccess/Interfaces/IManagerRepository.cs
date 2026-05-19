using DataAccess.Entities;
using StudentConferenceApp.DAL.Entities;
using System.Threading.Tasks;

namespace StudentConferenceApp.DAL.Repositories.Interfaces
{
    public interface IManagerRepository : IGenericRepository<Manager>
    {
        Task<Manager?> GetManagerByParticipantIdAsync(int participantId);
    }
}
