namespace Presentation.Pages.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.Interfaces;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminDashboardModel : PageModel
{
    private readonly IParticipantService _participantService;

    public AdminDashboardModel(IParticipantService participantService)
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
