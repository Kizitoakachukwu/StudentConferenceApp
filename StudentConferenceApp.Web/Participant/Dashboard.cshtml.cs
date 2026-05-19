using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using System.Security.Claims;

namespace StudentConferenceApp.Web.Pages.Participant
{
    [Authorize(Roles = "Participant")]
    public class DashboardModel : PageModel
    {
        private readonly IParticipantService _participantService;

        public DashboardModel(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public ParticipantDto? Participant { get; set; }
        public List<UploadedFileDto> UploadedFiles { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return;

            // For simplicity, we fetch by email or link via ApplicationUser
            // In real project, better to link Participant with ApplicationUserId
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return;

            // Get participant data (you may need to extend service to get by email)
            var allParticipants = await _participantService.GetAllParticipantsAsync();
            Participant = allParticipants.FirstOrDefault(p => p.Email == email);

            // TODO: Load uploaded files if you implement file upload
        }
    }

    // Simple DTO for uploaded files (you can expand later)
    public class UploadedFileDto
    {
        public string FileType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}