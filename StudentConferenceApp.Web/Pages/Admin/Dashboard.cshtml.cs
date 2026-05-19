using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace StudentConferenceApp.Web.Pages.Admin
{
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
        public List<ParticipantDto> RecentParticipants { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allParticipants = await _participantService.GetAllParticipantsAsync();

            TotalParticipants = allParticipants.Count;
            ApprovedParticipants = allParticipants.Count(p => p.Status == "Approved");
            PendingParticipants = allParticipants.Count(p => p.Status == "Pending");
            RejectedParticipants = allParticipants.Count(p => p.Status == "Rejected");

            // Show latest 5 registrations
            RecentParticipants = allParticipants
                .OrderByDescending(p => p.RegistrationDate)
                .Take(5)
                .ToList();
        }

        private string GetStatusBadge(string status)
        {
            return status?.ToLower() switch
            {
                "approved" => "success",
                "pending" => "warning",
                "rejected" => "danger",
                _ => "secondary"
            };
        }
    }
}
