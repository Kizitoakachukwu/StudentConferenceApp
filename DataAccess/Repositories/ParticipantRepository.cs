using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudentConferenceApp.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace StudentConferenceApp.DAL.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _context;

        public ParticipantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
           return await _context.Database.BeginTransactionAsync();
        }

        // ==================== Generic Methods ====================

        public async Task<Participant?> GetByIdAsync(int id)
        {
            return await _context.Participants.FindAsync(id);
        }

        public async Task<IEnumerable<Participant>> GetAllAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        public async Task<IEnumerable<Participant>> FindAsync(Expression<Func<Participant, bool>> predicate)
        {
            return await _context.Participants.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Participant entity)
        {
            await _context.Participants.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Participant> entities)
        {
            await _context.Participants.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Update(Participant entity)
        {
            _context.Participants.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(Participant entity)
        {
            _context.Participants.Remove(entity);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<Participant> entities)
        {
            _context.Participants.RemoveRange(entities);
            _context.SaveChanges();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Participant, bool>> predicate)
        {
            return await _context.Participants.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<Participant, bool>> predicate)
        {
            return await _context.Participants.CountAsync(predicate);
        }

        // ==================== Specific Methods ====================

        public async Task<Participant?> GetParticipantWithDetailsAsync(int id)
        {
            // No more Manager, Section, UploadedFiles, GeneratedDocuments
            return await _context.Participants
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IEnumerable<Participant>> GetAllParticipantsWithDetailsAsync()
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .Include(p => p.ApplicationUser)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<Participant>> GetParticipantsByStatusAsync(string status)
        {
            return await _context.Participants
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Participant>> GetParticipantsBySectionAsync(string sectionName)
        {
            return await _context.Participants
                .Where(p => p.SectionName == sectionName)
                .ToListAsync();
        }

        public async Task<Participant?> GetParticipantByEmailAsync(string email)
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Participant?> GetByIdWithManagerAsync(int id)
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Participants.AnyAsync(p => p.Email == email);
        }

        public async Task<IEnumerable<Participant>> GetAllParticipantsAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        public async Task<IEnumerable<Participant>> GetParticipantsSupervisedByManagerUserAsync(string identityUserId)
        {
            return await _context.Participants
                .Include(p => p.Manager)
                .Where(p => p.Manager != null && p.Manager.LinkedUserId == identityUserId)
                .OrderByDescending(p => p.RegistrationDate)
                .ToListAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
