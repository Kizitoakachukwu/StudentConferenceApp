using Microsoft.AspNetCore.Mvc;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.BLL.DTOs;

namespace PresentationLayer.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IParticipantService _participantService;

        public ParticipantController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        public async Task<IActionResult> Index()
        {
            var participants = await _participantService.GetAllParticipantsAsync();
            return View(participants);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new ParticipantRegisterDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ParticipantRegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _participantService.RegisterParticipantAsync(model);
            return RedirectToAction(nameof(Index));
        }
    }
}
