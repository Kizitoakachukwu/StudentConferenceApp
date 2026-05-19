using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.Interfaces;

namespace Presentation.Pages.Models;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly IParticipantService _participantService;

    public DashboardModel(IParticipantService participantService)
    {
        _participantService = participantService;
    }

    public int TotalParticipants { get; set; }
    public int ApprovedParticipants { get; set; }
    public int PendingParticipants { get; set; }
    public int RejectedParticipants { get; set; }

    public async Task OnGetAsync()
    {
        var all = await _participantService.GetAllParticipantsAsync();

        TotalParticipants = all.Count;
        ApprovedParticipants = all.Count(p => p.Status == "Approved");
        PendingParticipants = all.Count(p => p.Status == "Pending");
        RejectedParticipants = all.Count(p => p.Status == "Rejected");
    }
}
