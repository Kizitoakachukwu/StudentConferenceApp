using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentConferenceApp.DAL.Repositories.Interfaces
{
    public interface IParticipantRepository : IGenericRepository<Participant>
    {
        Task<Participant?> GetParticipantWithDetailsAsync(int id);
        Task<IEnumerable<Participant>> GetAllParticipantsWithDetailsAsync();
        Task<IEnumerable<Participant>> GetParticipantsByStatusAsync(string status);
        Task<Participant?> GetParticipantByEmailAsync(string email);
        Task<Participant?> GetByIdWithManagerAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<IEnumerable<Participant>> GetAllParticipantsAsync();
        Task<IEnumerable<Participant>> GetParticipantsSupervisedByManagerUserAsync(string identityUserId);
        Task<IEnumerable<Participant>> GetParticipantsBySectionAsync(string sectionName);
        Task SaveChangeAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}