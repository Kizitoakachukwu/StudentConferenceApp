using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.DAL.Entities;

namespace Presentation.Pages.Models;

[Authorize(Roles = "Participant")]
public class ParticipantDashboardModel : PageModel
{
    private readonly IParticipantService _participantService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ParticipantDashboardModel(IParticipantService participantService, UserManager<ApplicationUser> userManager)
    {
        _participantService = participantService;
        _userManager = userManager;
    }

    public ParticipantDto? Participant { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToPage("/Login");

        if (user.ParticipantId == null)
            return RedirectToPage("/Participant/ConferenceRegistration");

        Participant = await _participantService.GetParticipantByEmail(user.Email);

        if (Participant == null)
            return RedirectToPage("/Participant/ConferenceRegistration");

        return Page();
    }
}
