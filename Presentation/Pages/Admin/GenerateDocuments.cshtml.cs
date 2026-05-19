using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace Presentation.Pages.Admin;

[Authorize(Roles = "Admin")]
public class GenerateDocumentsModel : PageModel
{
    private readonly IParticipantService _participantService;
    private readonly IDocumentService _documentService;

    public GenerateDocumentsModel(
        IParticipantService participantService,
        IDocumentService documentService)
    {
        _participantService = participantService;
        _documentService = documentService;
    }

    public List<ParticipantDto> Participants { get; set; } = new();

    [BindProperty]
    public int SelectedParticipantId { get; set; }

    public string? FinalReportPath { get; set; }
    public string? FinalReportPdfPath { get; set; }

    public async Task OnGetAsync()
    {
        Participants = await _participantService.GetAllParticipantsAsync();
    }

    public async Task<IActionResult> OnPostGenerateCertificateAsync()
    {
        var dto = await _participantService.GetParticipantByIdAsync(SelectedParticipantId);
        if (dto == null) return Page();

        await _documentService.GenerateCertificateAsync(dto);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostGenerateInvitationAsync()
    {
        var dto = await _participantService.GetParticipantByIdAsync(SelectedParticipantId);
        if (dto == null) return Page();

        await _documentService.GenerateInvitationLetterAsync(dto);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostGenerateFinalReportAsync()
    {
        FinalReportPath = await _documentService.GenerateFinalReportAsync();
        Participants = await _participantService.GetAllParticipantsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostGenerateFinalReportPdfAsync()
    {
        FinalReportPdfPath = await _documentService.GenerateFinalReportPdfAsync();
        Participants = await _participantService.GetAllParticipantsAsync();
        return Page();
    }
}
