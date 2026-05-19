using System.ComponentModel.DataAnnotations;

namespace StudentConferenceApp.BLL.DTOs;

public class AccountRegisterDto
{
    [Required, EmailAddress, Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6), Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required, Display(Name = "Full last name")]
    public string FullLastName { get; set; } = string.Empty;
}
