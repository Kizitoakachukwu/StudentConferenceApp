using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace StudentConferenceApp.DataAccess.Seeding;

/// <summary>Creates one sample manager when the database has participants but no manager rows yet.</summary>
public static class ManagerSeeding
{
    public static async Task EnsureSampleManagerAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Managers.AnyAsync(cancellationToken))
            return;

        var participantId = await db.Participants
            .OrderBy(p => p.Id)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (participantId == 0)
            return;

        db.Managers.Add(new Manager
        {
            ParticipantId = participantId,
            FullName = "Dr. Marie Curie",
            PlaceOfEmployment = "Faculty of Science — University",
            AcademicDegree = "Ph.D.",
            Position = "Research supervisor"
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
