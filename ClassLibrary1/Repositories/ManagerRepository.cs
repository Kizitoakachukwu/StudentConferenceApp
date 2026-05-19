using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StudentConferenceApp.DAL.Repositories
{
    public class ManagerRepository : GenericRepository<Manager>, IManagerRepository
    {
        public ManagerRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Manager?> GetManagerByParticipantIdAsync(int participantId)
        {
            return await _context.Managers
                .FirstOrDefaultAsync(m => m.ParticipantId == participantId);
        }

        public async Task<Manager?> GetManagerWithParticipantAsync(int managerId)
        {
            return await _context.Managers
                .Include(m => m.Participant)
                .FirstOrDefaultAsync(m => m.Id == managerId);
        }
    }
}
