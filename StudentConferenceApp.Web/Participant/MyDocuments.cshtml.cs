using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.Interfaces;
using System.Security.Claims;

namespace StudentConferenceApp.Web.Pages.Participant
{
    [Authorize(Roles = "Participant")]
    public class MyDocumentsModel : PageModel
    {
        private readonly IParticipantService _participantService;
        private readonly IDocumentService _documentService;

        public MyDocumentsModel(IParticipantService participantService, IDocumentService documentService)
        {
            _participantService = participantService;
            _documentService = documentService;
        }

        public List<DocumentDto> Documents { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return;

            var participants = await _participantService.GetAllParticipantsAsync();
            var currentParticipant = participants.FirstOrDefault(p => p.Email == email);

            if (currentParticipant != null)
            {
                // In real implementation, you should have a method to get documents by participant
                // For now we simulate
                Documents = new List<DocumentDto>
                {
                    new DocumentDto
                    {
                        DocumentType = "Invitation Letter",
                        GeneratedAt = DateTime.Now.AddDays(-2),
                        FilePath = $"/documents/invitations/inv_{currentParticipant.Id}.pdf"
                    }
                };
            }
        }
    }

    public class DocumentDto
    {
        public string DocumentType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}