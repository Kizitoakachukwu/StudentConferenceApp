using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace StudentConferenceApp.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ParticipantsModel : PageModel
    {
        private readonly IParticipantService _participantService;

        public ParticipantsModel(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public List<ParticipantDto> Participants { get; set; } = new();

        public async Task OnGetAsync()
        {
            Participants = await _participantService.GetAllParticipantsAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            await _participantService.ApproveParticipantAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            await _participantService.RejectParticipantAsync(id);
            return RedirectToPage();
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
