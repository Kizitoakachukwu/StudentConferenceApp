using StudentConferenceApp.BLL.DTOs;

namespace StudentConferenceApp.BLL.Interfaces;

public interface ISectionService
{
    Task<IEnumerable<SectionDto>> GetAllSectionsAsync();
    Task<string?> GetSectionNameAsync(int sectionId);
}
