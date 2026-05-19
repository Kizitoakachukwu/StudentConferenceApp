using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.DAL.Entities;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace StudentConferenceApp.BLL.Services;

public class ParticipantService : IParticipantService
{
    private static readonly HashSet<string> PaperExtensions = new(StringComparer.OrdinalIgnoreCase) { ".docx" };
    private static readonly HashSet<string> PresentationExtensions = new(StringComparer.OrdinalIgnoreCase) { ".ppt", ".pptx", ".pdf" };

    private readonly IParticipantRepository _participantRepository;
    private readonly IDocumentService _documentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISectionService _sectionService;
    private readonly IWebHostEnvironment _env;

    public ParticipantService(
        IParticipantRepository participantRepository,
        IDocumentService documentService,
        UserManager<ApplicationUser> userManager,
        ISectionService sectionService,
        IWebHostEnvironment env)
    {
        _participantRepository = participantRepository;
        _documentService = documentService;
        _userManager = userManager;
        _sectionService = sectionService;
        _env = env;
    }

    public async Task RegisterAccountAsync(AccountRegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullLastName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Participant");
    }

    public async Task<int> SubmitConferenceRegistrationAsync(string userId, ParticipantRegisterDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");

        if (user.ParticipantId != null)
            throw new InvalidOperationException("Conference registration is already completed.");

        ValidateUploads(dto);

        var sectionName = await _sectionService.GetSectionNameAsync(dto.SectionId)
            ?? throw new InvalidOperationException("Invalid section.");

        var email = user.Email ?? throw new InvalidOperationException("User email is missing.");

        if (await _participantRepository.IsEmailExistsAsync(email))
            throw new InvalidOperationException("A conference profile for this email already exists.");

        await using var transaction = await _participantRepository.BeginTransactionAsync();

        try
        {
            var paperPath = await SaveFileAsync(dto.PaperFile!, "papers");
            var presPath = await SaveFileAsync(dto.PresentationFile!, "presentations");

            var participant = new Participant
            {
                FullName = dto.SpeakerLastName,
                Institution = dto.Institution,
                University = dto.Institution,
                PhoneNumber = dto.Phone,
                Email = email,
                PaperTitle = dto.PaperTitle,
                SectionId = dto.SectionId,
                SectionName = sectionName,
                ParticipationMethod = dto.ParticipationMethod,
                RegistrationDate = DateTime.UtcNow,
                Status = "Pending",
                DocumentPath = paperPath,
                PresentationPath = presPath ?? string.Empty,
                Password = string.Empty
            };

            if (!string.IsNullOrWhiteSpace(dto.ManagerFullName))
            {
                participant.Manager = new Manager
                {
                    FullName = dto.ManagerFullName.Trim(),
                    PlaceOfEmployment = dto.ManagerEmployment?.Trim() ?? string.Empty,
                    AcademicDegree = dto.ManagerDegree?.Trim() ?? string.Empty,
                    Position = dto.ManagerPosition?.Trim() ?? string.Empty
                };
            }

            await _participantRepository.AddAsync(participant);

            user.ParticipantId = participant.Id;
            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
                throw new InvalidOperationException(string.Join(" ", update.Errors.Select(e => e.Description)));

            var zipPath = await _documentService.GenerateRegistrationDataReportAsync(participant.Id);
            participant.RegistrationReportZipPath = zipPath;
            _participantRepository.Update(participant);

            await _documentService.GenerateInvitationLetterAsync(ToDto(await _participantRepository.GetByIdWithManagerAsync(participant.Id) ?? participant));

            await transaction.CommitAsync();
            return participant.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ParticipantDto>> GetAllParticipantsAsync()
    {
        var participants = await _participantRepository.GetAllParticipantsAsync();
        return participants.Select(ToDto).ToList();
    }

    public async Task<List<ParticipantDto>> GetAllParticipantsWithDetailsAsync()
    {
        var participants = await _participantRepository.GetAllParticipantsWithDetailsAsync();
        return participants.Select(ToDto).ToList();
    }

    public async Task<ParticipantDto?> GetParticipantByIdAsync(int id)
    {
        var p = await _participantRepository.GetByIdWithManagerAsync(id);
        return p == null ? null : ToDto(p);
    }

    public async Task<bool> ApproveParticipantAsync(int id)
    {
        var p = await _participantRepository.GetByIdAsync(id);
        if (p == null) return false;

        p.Status = "Approved";
        _participantRepository.Update(p);

        await _documentService.GenerateCertificateAsync(ToDto(p));
        return true;
    }

    public async Task<bool> RejectParticipantAsync(int id)
    {
        var p = await _participantRepository.GetByIdAsync(id);
        if (p == null) return false;

        p.Status = "Rejected";
        _participantRepository.Update(p);
        return true;
    }

    public async Task<bool> IsEmailExistsAsync(string email) =>
        await _userManager.FindByEmailAsync(email) != null;

    public async Task<ParticipantDto?> GetParticipantByEmail(string? email)
    {
        if (string.IsNullOrEmpty(email)) return null;
        var p = await _participantRepository.GetParticipantByEmailAsync(email);
        return p == null ? null : ToDto(p);
    }

    public async Task<List<ParticipantDto>> GetSupervisedParticipantsAsync(string managerUserId)
    {
        var list = await _participantRepository.GetParticipantsSupervisedByManagerUserAsync(managerUserId);
        return list.Select(ToDto).ToList();
    }

    public async Task<bool> SetSupervisedParticipantStatusAsync(string managerUserId, int participantId, string status)
    {
        var supervised = (await _participantRepository.GetParticipantsSupervisedByManagerUserAsync(managerUserId)).ToList();
        if (supervised.All(x => x.Id != participantId))
            return false;

        return await SetParticipantStatusAsync(participantId, status);
    }

    public async Task<bool> SetParticipantStatusAsync(int participantId, string status)
    {
        var key = (status ?? string.Empty).Trim().ToUpperInvariant();
        var normalized = key switch
        {
            "PENDING" => "Pending",
            "APPROVED" => "Approved",
            "REJECTED" => "Rejected",
            _ => ""
        };
        if (normalized.Length == 0)
            return false;

        if (normalized == "Approved")
            return await ApproveParticipantAsync(participantId);

        if (normalized == "Rejected")
            return await RejectParticipantAsync(participantId);

        var p = await _participantRepository.GetByIdAsync(participantId);
        if (p == null) return false;
        p.Status = "Pending";
        _participantRepository.Update(p);
        return true;
    }

    private static void ValidateUploads(ParticipantRegisterDto dto)
    {
        if (dto.PaperFile == null || dto.PaperFile.Length == 0)
            throw new InvalidOperationException("Conference paper file is required.");
        if (dto.PresentationFile == null || dto.PresentationFile.Length == 0)
            throw new InvalidOperationException("Presentation file is required.");

        var paperExt = Path.GetExtension(dto.PaperFile.FileName);
        if (!PaperExtensions.Contains(paperExt))
            throw new InvalidOperationException("Paper must be a Word file in .docx format (Word 2007 or newer).");

        var presExt = Path.GetExtension(dto.PresentationFile.FileName);
        if (!PresentationExtensions.Contains(presExt))
            throw new InvalidOperationException("Presentation must be .ppt, .pptx, or .pdf.");
    }

    private async Task<string?> SaveFileAsync(IFormFile file, string folder)
    {
        if (file.Length == 0) return null;

        var uploadsFolder = Path.Combine(_env.WebRootPath, folder);
        Directory.CreateDirectory(uploadsFolder);

        var ext = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString("N") + ext;
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return "/" + folder.Replace('\\', '/') + "/" + fileName;
    }

    private static ParticipantDto ToDto(Participant p) =>
        new()
        {
            Id = p.Id,
            FullName = p.FullName,
            University = p.University,
            Institution = p.Institution,
            PaperTitle = p.PaperTitle,
            PhoneNumber = p.PhoneNumber,
            Email = p.Email,
            SectionName = p.SectionName,
            SectionId = p.SectionId,
            ParticipationMethod = p.ParticipationMethod,
            Status = p.Status,
            DocumentPath = p.DocumentPath,
            PresentationPath = p.PresentationPath,
            CertificatePath = p.CertificatePath,
            InvitationPath = p.InvitationPath,
            RegistrationReportZipPath = p.RegistrationReportZipPath,
            RegistrationDate = p.RegistrationDate,
            ManagerFullName = p.Manager?.FullName,
            ManagerEmployment = p.Manager?.PlaceOfEmployment,
            ManagerDegree = p.Manager?.AcademicDegree,
            ManagerPosition = p.Manager?.Position
        };
}
