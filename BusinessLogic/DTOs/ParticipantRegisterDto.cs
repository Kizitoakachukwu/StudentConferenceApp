using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace StudentConferenceApp.BLL.DTOs;

public class ParticipantRegisterDto
{
    [Required(ErrorMessage = "Full last name of the speaker is required")]
    [Display(Name = "Full last name (speaker)")]
    public string SpeakerLastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Educational institution is required")]
    [Display(Name = "Educational institution")]
    public string Institution { get; set; } = string.Empty;

    [Required(ErrorMessage = "Paper title is required")]
    [Display(Name = "Title (topic) of the paper")]
    public string PaperTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "Phone must be between 5 and 40 characters.")]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Section is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Section is required")]
    [Display(Name = "Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Participation method is required")]
    [Display(Name = "Participation method")]
    public string ParticipationMethod { get; set; } = string.Empty;

    // IFormFile + [Required] breaks jQuery Validate (file value is not read like text). Validate in the page handler and in ParticipantService.
    [ValidateNever]
    [Display(Name = "Conference paper (.docx)")]
    public IFormFile? PaperFile { get; set; }

    [ValidateNever]
    [Display(Name = "Presentation (.ppt, .pptx, .pdf)")]
    public IFormFile? PresentationFile { get; set; }

    [Display(Name = "Manager full last name (if any)")]
    public string? ManagerFullName { get; set; }

    [Display(Name = "Manager place of employment")]
    public string? ManagerEmployment { get; set; }

    [Display(Name = "Manager academic degree (if any)")]
    public string? ManagerDegree { get; set; }

    [Display(Name = "Manager position (if any)")]
    public string? ManagerPosition { get; set; }
}
