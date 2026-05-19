using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StudentConferenceApp.BLL.Interfaces;

namespace PresentationLayer.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IParticipantRepository _participantRepository;

        public DocumentController(IDocumentService documentService, IParticipantRepository participantRepository)
        {
            _documentService = documentService;
            _participantRepository = participantRepository;
        }

        public async Task<IActionResult> Invitation(int id)
        {
            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            var content = await _documentService.GenerateInvitationLetterAsync(participant);
            return Content(content, "text/plain");
        }

        public async Task<IActionResult> Certificate(int id)
        {
            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null)
                return NotFound();

            var content = await _documentService.GenerateCertificateAsync(participant);
            return Content(content, "text/plain");
        }
    }
}
