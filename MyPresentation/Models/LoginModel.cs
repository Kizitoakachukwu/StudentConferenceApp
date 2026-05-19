using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models.Account
{
    public class LoginModel
    {
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }
    }
}
