using Microsoft.EntityFrameworkCore;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace StudentConferenceApp.BLL.Services;

public class SectionService : ISectionService
{
    private readonly ApplicationDbContext _context;

    public SectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SectionDto>> GetAllSectionsAsync()
    {
        return await _context.Sections
            .OrderBy(s => s.Name)
            .Select(s => new SectionDto { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<string?> GetSectionNameAsync(int sectionId)
    {
        return await _context.Sections
            .Where(s => s.Id == sectionId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync();
    }
}
