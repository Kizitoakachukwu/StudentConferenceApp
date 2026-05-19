using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace StudentConferenceApp.Web.Pages;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
            return RedirectToPage("/Index");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return RedirectToPage("/Admin/Dashboard");

        if (await _userManager.IsInRoleAsync(user, "Manager"))
            return RedirectToPage("/Manager/Dashboard");

        if (await _userManager.IsInRoleAsync(user, "Participant"))
        {
            if (user.ParticipantId == null)
                return RedirectToPage("/Participant/ConferenceRegistration");
            return RedirectToPage("/Participant/Dashboard");
        }

        return RedirectToPage("/Index");
    }
}

public class LoginInputModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}
