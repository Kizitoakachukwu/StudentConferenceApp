using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StudentConferenceApp.DAL.Repositories
{
    public class ParticipantRepository : GenericRepository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<Participant?> GetParticipantWithDetailsAsync(int id)
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .Include(p => p.Section)
                .Include(p => p.UploadedFiles)
                .Include(p => p.GeneratedDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Participant>> GetAllParticipantsWithDetailsAsync()
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .Include(p => p.Section)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Participant>> GetParticipantsByStatusAsync(string status)
        {
            return await FindAsync(p => p.Status == status);
        }

        public async Task<Participant?> GetParticipantByEmailAsync(string email)
        {
            return await _context.Participants
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await ExistsAsync(p => p.Email == email);
        }
    }
}