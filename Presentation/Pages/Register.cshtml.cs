using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace StudentConferenceApp.Web.Pages;

public class RegisterModel : PageModel
{
    private readonly IParticipantService _participantService;

    public RegisterModel(IParticipantService participantService)
    {
        _participantService = participantService;
    }

    [BindProperty]
    public AccountRegisterDto Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _participantService.RegisterAccountAsync(Input);
            TempData["SuccessMessage"] = "Account created. Sign in to complete conference registration in your personal account.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
