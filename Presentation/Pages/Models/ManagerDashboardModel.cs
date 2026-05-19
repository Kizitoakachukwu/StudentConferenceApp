using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.DAL.Entities;

namespace Presentation.Pages.Models;

[Authorize(Roles = "Manager")]
public class ManagerDashboardModel : PageModel
{
    private readonly IParticipantService _participantService;
    private readonly IDocumentService _documentService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ManagerDashboardModel(
        IParticipantService participantService,
        IDocumentService documentService,
        UserManager<ApplicationUser> userManager)
    {
        _participantService = participantService;
        _documentService = documentService;
        _userManager = userManager;
    }

    public IReadOnlyList<ParticipantDto> Participants { get; private set; } = Array.Empty<ParticipantDto>();

    public async Task OnGetAsync()
    {
        Participants = await LoadAllParticipantsAsync();
    }

    public async Task<IActionResult> OnPostSetStatusAsync(int participantId, string status)
    {
        if (await _userManager.GetUserAsync(User) == null)
            return Challenge();

        var ok = await _participantService.SetParticipantStatusAsync(participantId, status);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Could not update status. Check the selected value and try again.";
            return RedirectToPage();
        }

        TempData["SuccessMessage"] = "Applicant status updated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetExportCsvAsync()
    {
        var list = await LoadAllParticipantsAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Speaker,Email,PaperTitle,Section,Status,Institution,Phone,Supervisor");
        foreach (var p in list)
        {
            sb.AppendLine(string.Join(',', new[]
            {
                CsvCell(p.FullName),
                CsvCell(p.Email),
                CsvCell(p.PaperTitle),
                CsvCell(p.SectionName),
                CsvCell(p.Status),
                CsvCell(p.Institution),
                CsvCell(p.PhoneNumber),
                CsvCell(p.ManagerFullName)
            }));
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(sb.ToString());
        var bytes = new byte[preamble.Length + body.Length];
        Buffer.BlockCopy(preamble, 0, bytes, 0, preamble.Length);
        Buffer.BlockCopy(body, 0, bytes, preamble.Length, body.Length);

        return File(bytes, "text/csv; charset=utf-8", $"participants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
    }

    public async Task<IActionResult> OnGetExportRosterAsync()
    {
        var list = await LoadAllParticipantsAsync();
        var sb = new StringBuilder();
        foreach (var p in list)
        {
            sb.AppendLine($"{p.FullName}\t{p.Email}\t{p.PaperTitle}\t{p.SectionName}\t{p.Status}\t{p.ManagerFullName}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/plain; charset=utf-8", $"participants-roster_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt");
    }

    public async Task<IActionResult> OnGetExportWordAsync()
    {
        var list = await LoadAllParticipantsAsync();
        if (list.Count == 0)
            return RedirectToPage();

        var bytes = await _documentService.GenerateParticipantRosterWordAsync(list);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"participants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.docx");
    }

    public async Task<IActionResult> OnGetExportPdfAsync()
    {
        var list = await LoadAllParticipantsAsync();
        if (list.Count == 0)
            return RedirectToPage();

        var bytes = await _documentService.GenerateParticipantRosterPdfAsync(list);
        return File(bytes, "application/pdf", $"participants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf");
    }

    public async Task<IActionResult> OnGetExportJsonAsync()
    {
        var list = await LoadAllParticipantsAsync();
        var payload = list.Select(p => new
        {
            p.Id,
            p.FullName,
            p.Email,
            p.PaperTitle,
            p.SectionName,
            p.Status,
            p.Institution,
            p.PhoneNumber,
            p.ParticipationMethod,
            Manager = new { p.ManagerFullName, p.ManagerEmployment, p.ManagerDegree, p.ManagerPosition }
        });

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var bytes = Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json; charset=utf-8", $"participants_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
    }

    private Task<List<ParticipantDto>> LoadAllParticipantsAsync() =>
        _participantService.GetAllParticipantsWithDetailsAsync();

    private static string CsvCell(string? value)
    {
        var s = value ?? string.Empty;
        if (s.Contains('"', StringComparison.Ordinal))
            s = s.Replace("\"", "\"\"", StringComparison.Ordinal);
        if (s.Contains(',', StringComparison.Ordinal) || s.Contains('\n', StringComparison.Ordinal) || s.Contains('\r', StringComparison.Ordinal))
            return $"\"{s}\"";
        return s;
    }
}
