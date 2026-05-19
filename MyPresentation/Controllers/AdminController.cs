using Microsoft.AspNetCore.Mvc;
using StudentConferenceApp.BLL.Interfaces;

namespace PresentationLayer.Controllers
{
    public class AdminController : Controller
    {
        private readonly IParticipantService _participantService;

        public AdminController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public async Task<IActionResult> Index()
        {
            var participants = await _participantService.GetAllParticipantsAsync();
            return View(participants);
        }

        public async Task<IActionResult> Details(int id)
        {
            var participant = await _participantService.GetParticipantByIdAsync(id);
            if (participant == null)
                return NotFound();

            return View(participant);
        }

        public async Task<IActionResult> Approve(int id)
        {
            await _participantService.ApproveParticipantAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            await _participantService.RejectParticipantAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
