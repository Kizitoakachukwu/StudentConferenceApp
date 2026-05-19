using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentConferenceApp.DataAccess.Seeding;

/// <summary>Reference data for conference sections (data access layer).</summary>
public static class SectionSeeding
{
    public static async Task EnsureDefaultSectionsAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Sections.AnyAsync(cancellationToken))
            return;

        db.Sections.AddRange(
            new Section { Name = "Information technologies" },
            new Section { Name = "Economics and management" },
            new Section { Name = "Natural sciences" },
            new Section { Name = "Engineering" });

        await db.SaveChangesAsync(cancellationToken);
    }
}
