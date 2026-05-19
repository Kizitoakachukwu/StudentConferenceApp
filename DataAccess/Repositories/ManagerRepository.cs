using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace StudentConferenceApp.DAL.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ApplicationDbContext _context;

        public ManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Manager?> GetByIdAsync(int id)
        {
            return await _context.Managers.FindAsync(id);
        }

        public async Task<IEnumerable<Manager>> GetAllAsync()
        {
            return await _context.Managers.ToListAsync();
        }

        public async Task AddAsync(Manager entity)
        {
            await _context.Managers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(Manager entity)
        {
            _context.Managers.Update(entity);
        }

        public void Remove(Manager entity)
        {
            _context.Managers.Remove(entity);
        }

        public async Task<Manager?> GetManagerByParticipantIdAsync(int participantId)
        {
            return await _context.Managers
                .FirstOrDefaultAsync(m => m.ParticipantId == participantId);
        }

        // Implement other IGenericRepository methods similarly...
        public Task<IEnumerable<Manager>> FindAsync(System.Linq.Expressions.Expression<Func<Manager, bool>> predicate)
            => Task.FromResult(_context.Managers.Where(predicate).AsEnumerable());

        public Task AddRangeAsync(IEnumerable<Manager> entities) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<Manager, bool>> predicate)
            => _context.Managers.AnyAsync(predicate);
        public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Manager, bool>> predicate)
            => _context.Managers.CountAsync(predicate);
        public Task RemoveRange(IEnumerable<Manager> entities) => throw new NotImplementedException();

        void IGenericRepository<Manager>.RemoveRange(IEnumerable<Manager> entities)
        {
            throw new NotImplementedException();
        }
    }
}