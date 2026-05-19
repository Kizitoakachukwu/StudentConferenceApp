using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.DAL.Entities;

namespace StudentConferenceApp.Web.Pages.Participant;

[Authorize(Roles = "Participant")]
[RequestSizeLimit(104_857_600)]
[RequestFormLimits(MultipartBodyLengthLimit = 104_857_600)]
public class ConferenceRegistrationModel : PageModel
{
    private readonly IParticipantService _participantService;
    private readonly ISectionService _sectionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ConferenceRegistrationModel(
        IParticipantService participantService,
        ISectionService sectionService,
        UserManager<ApplicationUser> userManager)
    {
        _participantService = participantService;
        _sectionService = sectionService;
        _userManager = userManager;
    }

    [BindProperty]
    public ParticipantRegisterDto Input { get; set; } = new();

    public List<SelectListItem> Sections { get; set; } = new();

    public string? AccountEmail { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Login");
        if (user.ParticipantId != null)
            return RedirectToPage("/Participant/Dashboard");

        AccountEmail = user.Email;
        await LoadSectionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Login");
        if (user.ParticipantId != null)
            return RedirectToPage("/Participant/Dashboard");

        AccountEmail = user.Email;
        await LoadSectionsAsync();

        if (Input.PaperFile is not { Length: > 0 })
            ModelState.AddModelError("Input.PaperFile", "Conference paper file (.docx) is required.");
        if (Input.PresentationFile is not { Length: > 0 })
            ModelState.AddModelError("Input.PresentationFile", "Presentation file is required.");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _participantService.SubmitConferenceRegistrationAsync(user.Id, Input);
            TempData["SuccessMessage"] = "Conference registration submitted. A registration report (ZIP) was generated.";
            return RedirectToPage("/Participant/Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }

    private async Task LoadSectionsAsync()
    {
        var sections = await _sectionService.GetAllSectionsAsync();
        Sections = sections
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
            .ToList();
    }
}
